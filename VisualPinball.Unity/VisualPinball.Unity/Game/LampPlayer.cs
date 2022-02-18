﻿// Visual Pinball Engine
// Copyright (C) 2022 freezy and VPE Team
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
using UnityEngine;
using VisualPinball.Engine.Game.Engines;
using VisualPinball.Engine.Math;
using Color = UnityEngine.Color;
using NLog;
using Logger = NLog.Logger;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VisualPinball.Unity
{
	public class LampPlayer
	{
		/// <summary>
		/// List of all registered lamp APIs.
		/// </summary>
		private readonly Dictionary<ILampDeviceComponent, IApiLamp> _lamps = new Dictionary<ILampDeviceComponent, IApiLamp>();

		/// <summary>
		/// Links the GLE's IDs to the lamps.
		/// </summary>
		private readonly Dictionary<string, List<ILampDeviceComponent>> _lampAssignments = new Dictionary<string, List<ILampDeviceComponent>>();

		public List<ILampDeviceComponent> LampDevice(string id) => _lampAssignments[id];

		/// <summary>
		/// Links the GLE's IDs to the mappings.
		/// </summary>
		private readonly Dictionary<string, Dictionary<ILampDeviceComponent, LampMapping>> _lampMappings = new Dictionary<string, Dictionary<ILampDeviceComponent, LampMapping>>();

		private Player _player;
		private TableComponent _tableComponent;
		private IGamelogicEngine _gamelogicEngine;

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		internal IApiLamp Lamp(ILampDeviceComponent component)
			=> _lamps.ContainsKey(component) ? _lamps[component] : null;

		internal Dictionary<string, LampState> LampStatuses { get; } = new Dictionary<string, LampState>();
		internal void RegisterLamp(ILampDeviceComponent component, IApiLamp lampApi) => _lamps[component] = lampApi;

		public void Awake(Player player, TableComponent tableComponent, IGamelogicEngine gamelogicEngine)
		{
			_player = player;
			_tableComponent = tableComponent;
			_gamelogicEngine = gamelogicEngine;
		}

		public void OnStart()
		{
			if (_gamelogicEngine != null) {
				var config = _tableComponent.MappingConfig;
				_lampAssignments.Clear();
				_lampMappings.Clear();
				foreach (var lampMapping in config.Lamps) {

					if (lampMapping.Device == null) {
						Logger.Warn($"Ignoring unassigned lamp \"{lampMapping.Id}\".");
						continue;
					}

					AssignLampMapping(lampMapping);

					// turn it off

					if (_lamps.ContainsKey(lampMapping.Device)) {
						_lamps[lampMapping.Device].OnLamp(LampState.Default);
					}
				}

				if (_lampAssignments.Count > 0) {
					_gamelogicEngine.OnLampChanged += HandleLampEvent;
					_gamelogicEngine.OnLampsChanged += HandleLampsEvent;
				}
			}
		}

		public void HandleLampEvent(LampEventArgs lampEvent)
		{
			Logger.Debug($"lamp {lampEvent.Id}: {lampEvent.Value}");
			if (_lampAssignments.ContainsKey(lampEvent.Id)) {
				foreach (var component in _lampAssignments[lampEvent.Id]) {
					var mapping = _lampMappings[lampEvent.Id][component];
					if (mapping.Source != lampEvent.Source || mapping.IsCoil != lampEvent.IsCoil) {
						// so, if we have a coil here that happens to have the same name as a lamp,
						// or a GI light with the same name as an other lamp, skip.
						continue;
					}
					if (_lamps.ContainsKey(component)) {
						var lamp = _lamps[component];
						var status = LampStatuses[lampEvent.Id];
						// var intensity = status.Intensity;
						// var channel = ColorChannel.Alpha;

						switch (mapping.Type) {
							case LampType.SingleOnOff:
								status.IsOn = lampEvent.Value > 0;
								//intensity = status.IsOn ? 1 : 0;
								break;

							case LampType.Rgb:
								status.Intensity = lampEvent.Value / 255f; // todo test
								//intensity = status.Intensity;
								break;

							case LampType.RgbMulti:
								status.SetChannel(mapping.Channel, lampEvent.Value / 255f); // todo test
								//channel = mapping.Channel;
								//intensity = lampEvent.Value / 255f;
								break;

							case LampType.SingleFading:
								status.Intensity = lampEvent.Value / mapping.FadingSteps;
								//intensity = status.Intensity;
								break;

							default:
								Logger.Error($"Unknown mapping type \"{mapping.Type}\" of lamp ID {lampEvent.Id} for light {component}.");
								break;
						}

						Logger.Debug($"lamp {lampEvent.Id}: {status}");
						LampStatuses[lampEvent.Id] = status;
						lamp.OnLamp(status);
					}
				}

			} else {
				LampStatuses[lampEvent.Id] = new LampState(lampEvent.Value);
			}

#if UNITY_EDITOR
			RefreshUI();
#endif
		}

		public void OnDestroy()
		{
			if (_lampAssignments.Count > 0 && _gamelogicEngine != null) {
				_gamelogicEngine.OnLampChanged -= HandleLampEvent;
				_gamelogicEngine.OnLampsChanged -= HandleLampsEvent;
			}
		}

		private void AssignLampMapping(LampMapping lampMapping)
		{
			var id = lampMapping.Id;
			if (!_lampAssignments.ContainsKey(id)) {
				_lampAssignments[id] = new List<ILampDeviceComponent>();
			}
			if (!_lampMappings.ContainsKey(id)) {
				_lampMappings[id] = new Dictionary<ILampDeviceComponent, LampMapping>();
			}
			_lampAssignments[id].Add(lampMapping.Device);
			_lampMappings[id][lampMapping.Device] = lampMapping;
			LampStatuses[id] = new LampState(0f);
		}

		private void HandleLampsEvent(object sender, LampsEventArgs lampsEvent)
		{
			foreach (var lampEvent in lampsEvent.LampsChanged) {
				HandleLampEvent(lampEvent);
			}
		}

		private void HandleLampEvent(object sender, LampEventArgs lampEvent)
		{
			HandleLampEvent(lampEvent);
		}

#if UNITY_EDITOR
		private void RefreshUI()
		{
			if (!_player.UpdateDuringGamplay) {
				return;
			}

			foreach (var manager in (EditorWindow[])Resources.FindObjectsOfTypeAll(Type.GetType("VisualPinball.Unity.Editor.LampManager, VisualPinball.Unity.Editor"))) {
				manager.Repaint();
			}
		}
#endif
	}
}
