﻿// Visual Pinball Engine
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

using System.Collections.Generic;
using NLog;
using UnityEngine;
using Logger = NLog.Logger;

namespace VisualPinball.Unity
{
	public class DisplayPlayer
	{
		private IGamelogicEngine _gamelogicEngine;
		private readonly Dictionary<string, DisplayComponent> _displayGameObjects = new Dictionary<string, DisplayComponent>();

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public void Awake(IGamelogicEngine gamelogicEngine)
		{
			_gamelogicEngine = gamelogicEngine;
			_gamelogicEngine.OnDisplaysAvailable += HandleDisplayAvailable;
			_gamelogicEngine.OnDisplayFrame += HandleFrameEvent;

			var dmds = Object.FindObjectsOfType<DisplayComponent>();
			foreach (var dmd in dmds) {
				Logger.Info($"[Player] Display \"{dmd.Id}\" connected.");
				_displayGameObjects[dmd.Id] = dmd;
			}
		}

		private void HandleDisplayAvailable(object sender, AvailableDisplays availableDisplays)
		{
			foreach (var display in availableDisplays.Displays) {
				if (_displayGameObjects.ContainsKey(display.Id)) {
					Logger.Info($"Updating display \"{display.Id}\" to {display.Width}x{display.Height}");
					_displayGameObjects[display.Id].UpdateDimensions(display.Width, display.Height, display.FlipX);
					_displayGameObjects[display.Id].Clear();

				} else {
					Logger.Error($"Cannot find DMD game object for display \"{display.Id}\"");
				}
			}
		}

		private void HandleFrameEvent(object sender, DisplayFrameData e)
		{
			if (_displayGameObjects.ContainsKey(e.Id)) {
				_displayGameObjects[e.Id].UpdateFrame(e.Format, e.Data);
			}
		}

		public void OnDestroy()
		{
			_gamelogicEngine.OnDisplaysAvailable -= HandleDisplayAvailable;
			_gamelogicEngine.OnDisplayFrame -= HandleFrameEvent;
		}
	}
}
