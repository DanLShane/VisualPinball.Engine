// Visual Pinball Engine
// Copyright (C) 2021 freezy and VPE Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VisualPinball.Engine.Game.Engines;

namespace VisualPinball.Unity.Editor
{
	public class SwitchListViewItemRenderer : ListViewItemRenderer<SwitchListData, GamelogicEngineSwitch, IApiSwitchStatus>
	{
		protected override List<GamelogicEngineSwitch> GleItems => _gleSwitches;
		protected override GamelogicEngineSwitch InstantiateGleItem(string id) => new GamelogicEngineSwitch(id);
		protected override Texture2D StatusIcon(IApiSwitchStatus status) => Icons.Switch(status.IsSwitchClosed, IconSize.Small, status.IsSwitchClosed ? IconColor.Orange : IconColor.Gray);

		private struct InputSystemEntry
		{
			public string ActionMapName;
			public string ActionName;
		}

		private enum SwitchListColumn
		{
			Id = 0,
			Nc = 1,
			Description = 2,
			Source = 3,
			Element = 4,
			PulseDelay = 5,
		}

		private readonly List<GamelogicEngineSwitch> _gleSwitches;
		private readonly InputManager _inputManager;

		private readonly ObjectReferencePicker<ISwitchDeviceComponent> _devicePicker;
		private readonly TableComponent _tableComponent;

		public SwitchListViewItemRenderer(List<GamelogicEngineSwitch> gleSwitches, TableComponent tableComponent, InputManager inputManager)
		{
			_gleSwitches = gleSwitches;
			_inputManager = inputManager;
			_devicePicker = new ObjectReferencePicker<ISwitchDeviceComponent>("Switch Devices", tableComponent, false);
			_tableComponent = tableComponent;
		}

		public void Render(TableComponent tableComponent, SwitchListData data, Rect cellRect, int column, Action<SwitchListData> updateAction)
		{
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			var switchStatuses = Application.isPlaying
				? tableComponent.gameObject.GetComponent<Player>()?.SwitchStatuses
				: null;
			switch ((SwitchListColumn)column)
			{
				case SwitchListColumn.Id:
					RenderId(switchStatuses, ref data.Id, id => data.Id = id, data, cellRect, updateAction);
					break;
				case SwitchListColumn.Nc:
					RenderNc(data, cellRect, updateAction);
					break;
				case SwitchListColumn.Description:
					RenderDescription(data, cellRect, updateAction);
					break;
				case SwitchListColumn.Source:
					RenderSource(data, cellRect, updateAction);
					break;
				case SwitchListColumn.Element:
					RenderElement(data, cellRect, updateAction);
					break;
				case SwitchListColumn.PulseDelay:
					RenderPulseDelay(data, cellRect, updateAction);
					break;
			}
			EditorGUI.EndDisabledGroup();
		}

		protected override void OnIconClick(SwitchListData data, bool pressedDown)
		{
			var gle = _tableComponent.GetComponent<IGamelogicEngine>();
			var player = _tableComponent.GetComponent<Player>();
			gle?.Switch(data.Id, pressedDown);
			if (player != null && player.SwitchStatuses.ContainsKey(data.Id)) {
				player.SwitchStatuses[data.Id].IsSwitchClosed = pressedDown;
			}
		}

		private void RenderNc(SwitchListData switchListData, Rect cellRect, Action<SwitchListData> updateAction)
		{
			// don't render for constants
			if (switchListData.Source == SwitchSource.Constant) {
				return;
			}

			// check if it's linked to a switch device, and whether the switch device handles no/nc itself
			var switchDefault = switchListData.Source == SwitchSource.Playfield
				? switchListData.Device?.SwitchDefault ?? SwitchDefault.Configurable
				: SwitchDefault.Configurable;

			// if it handles it itself, just render the checkbox
			if (switchDefault != SwitchDefault.Configurable) {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.Toggle(cellRect, switchDefault == SwitchDefault.NormallyClosed);
				EditorGUI.EndDisabledGroup();
				return;
			}

			// otherwise, let the user toggle
			EditorGUI.BeginChangeCheck();
			var value = EditorGUI.Toggle(cellRect, switchListData.NormallyClosed);
			if (EditorGUI.EndChangeCheck()) {
				switchListData.NormallyClosed = value;
				updateAction(switchListData);
			}
		}

		private void RenderSource(SwitchListData switchListData, Rect cellRect, Action<SwitchListData> updateAction)
		{
			EditorGUI.BeginChangeCheck();
			var source = (SwitchSource)EditorGUI.EnumPopup(cellRect, switchListData.Source);
			if (EditorGUI.EndChangeCheck()) {
				switchListData.Source = source;
				updateAction(switchListData);
			}
		}

		private void RenderElement(SwitchListData switchListData, Rect cellRect, Action<SwitchListData> updateAction)
		{
			switch (switchListData.Source)
			{
				case SwitchSource.InputSystem:
					cellRect = RenderIcon(switchListData, cellRect);
					RenderInputSystemElement(switchListData, cellRect, updateAction);
					break;

				case SwitchSource.Playfield:
					RenderDevice(switchListData, cellRect, updateAction);

					break;

				case SwitchSource.Constant:
					cellRect = RenderIcon(switchListData, cellRect);
					RenderConstantElement(switchListData, cellRect, updateAction);
					break;
			}
		}

		private void RenderInputSystemElement(SwitchListData switchListData, Rect cellRect, Action<SwitchListData> updateAction)
		{
			var inputSystemList = new List<InputSystemEntry>();
			var tmpIndex = 0;
			var selectedIndex = -1;
			var options = new List<string>();

			foreach (var actionMapName in _inputManager.GetActionMapNames())
			{
				if (options.Count > 0)
				{
					options.Add("");
					inputSystemList.Add(new InputSystemEntry());
					tmpIndex++;
				}

				foreach (var actionName in _inputManager.GetActionNames(actionMapName))
				{
					inputSystemList.Add(new InputSystemEntry
					{
						ActionMapName = actionMapName,
						ActionName = actionName
					});

					options.Add(actionName.Replace('/', '\u2215'));

					if (actionMapName == switchListData.InputActionMap && actionName == switchListData.InputAction)
					{
						selectedIndex = tmpIndex;
					}

					tmpIndex++;
				}
			}

			EditorGUI.BeginChangeCheck();
			var index = EditorGUI.Popup(cellRect, selectedIndex, options.ToArray());
			if (EditorGUI.EndChangeCheck())
			{
				switchListData.InputActionMap = inputSystemList[index].ActionMapName;
				switchListData.InputAction = inputSystemList[index].ActionName;
				updateAction(switchListData);
			}
		}

		private void RenderConstantElement(SwitchListData switchListData, Rect cellRect, Action<SwitchListData> updateAction)
		{
			EditorGUI.BeginChangeCheck();
			var constVal = (SwitchConstant)EditorGUI.EnumPopup(cellRect, switchListData.Constant);
			if (EditorGUI.EndChangeCheck()) {
				switchListData.Constant = constVal;
				updateAction(switchListData);
			}
		}

		protected override void RenderDeviceElement(SwitchListData switchListData, Rect cellRect, Action<SwitchListData> updateAction)
		{
			_devicePicker.Render(cellRect, switchListData.Device, component => {
				switchListData.Device = component;
				UpdateDeviceItem(switchListData);
				updateAction(switchListData);
			});
		}

		private void RenderPulseDelay(SwitchListData switchListData, Rect cellRect, Action<SwitchListData> updateAction)
		{
			if (switchListData.Source == SwitchSource.Playfield && switchListData.Device != null) {
				var switchable = switchListData.Device.AvailableSwitches.FirstOrDefault(sw => sw.Id == switchListData.DeviceItem);
				if (switchable != null && switchable.IsPulseSwitch) {
					var labelRect = cellRect;
					labelRect.x += labelRect.width - 20;
					labelRect.width = 20;

					var intFieldRect = cellRect;
					intFieldRect.width -= 25;

					EditorGUI.BeginChangeCheck();
					var pulse = EditorGUI.IntField(intFieldRect, switchListData.PulseDelay);
					if (EditorGUI.EndChangeCheck())
					{
						switchListData.PulseDelay = pulse;
						updateAction(switchListData);
					}

					EditorGUI.LabelField(labelRect, "ms");
				}
			}
		}

		protected override Texture GetIcon(SwitchListData switchListData)
		{
			Texture2D icon = null;

			switch (switchListData.Source) {
				case SwitchSource.Playfield: {
					if (switchListData.Device != null) {
						icon = Icons.ByComponent(switchListData.Device, IconSize.Small);
					}
					break;
				}
				case SwitchSource.Constant:
					icon = Icons.Switch(switchListData.Constant == SwitchConstant.Closed, IconSize.Small);
					break;

				case SwitchSource.InputSystem:
					icon = Icons.Key(IconSize.Small);
					break;
			}

			return icon;
		}
	}
}
