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

using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VisualPinball.Engine.Game.Engines;
using VisualPinball.Engine.Math;
using VisualPinball.Engine.VPT.Table;
using VisualPinball.Unity.Editor;

namespace VisualPinball.Unity.Test
{
	public class LampPopulationTests
	{
		[Test]
		public void ShouldMapALampWithTheSameName()
		{
			var table = new TableBuilder()
				.AddLight("some_light")
				.Build();

			var go = VpxImportEngine.ImportIntoScene(table, options: ConvertOptions.SkipNone);
			var tableComponent = go.GetComponent<TableComponent>();

			var gameEngineLamps = new[] {
				new GamelogicEngineLamp("some_light") { Description = "Some Light"}
			};

			tableComponent.MappingConfig.Clear();
			tableComponent.MappingConfig.PopulateLamps(gameEngineLamps, tableComponent);

			tableComponent.MappingConfig.Lamps.Should().HaveCount(1);
			tableComponent.MappingConfig.Lamps[0].Id.Should().Be("some_light");
			tableComponent.MappingConfig.Lamps[0].Description.Should().Be("Some Light");
			tableComponent.MappingConfig.Lamps[0].Device.name.Should().Be("some_light");
			tableComponent.MappingConfig.Lamps[0].DeviceItem.Should().Be(LightComponent.LampIdDefault);
		}

		[Test]
		public void ShouldMapALampWithTheSameNumericalId()
		{
			var table = new TableBuilder()
				.AddLight("l42")
				.Build();

			var go = VpxImportEngine.ImportIntoScene(table, options: ConvertOptions.SkipNone);
			var tableComponent = go.GetComponent<TableComponent>();

			var gameEngineLamps = new[] {
				new GamelogicEngineLamp("42") { Description = "Light 42"}
			};

			tableComponent.MappingConfig.Clear();
			tableComponent.MappingConfig.PopulateLamps(gameEngineLamps, tableComponent);

			tableComponent.MappingConfig.Lamps.Should().HaveCount(1);
			tableComponent.MappingConfig.Lamps[0].Id.Should().Be("42");
			tableComponent.MappingConfig.Lamps[0].Description.Should().Be("Light 42");
			tableComponent.MappingConfig.Lamps[0].Device.name.Should().Be("l42");
			tableComponent.MappingConfig.Lamps[0].DeviceItem.Should().Be(LightComponent.LampIdDefault);
		}

		[Test]
		public void ShouldMapALampWithViaRegex()
		{
			var table = new TableBuilder()
				.AddLight("lamp_foobar_name")
				.Build();

			var go = VpxImportEngine.ImportIntoScene(table, options: ConvertOptions.SkipNone);
			var tableComponent = go.GetComponent<TableComponent>();

			var gameEngineLamps = new[] {
				new GamelogicEngineLamp("11") { Description = "Foobar", DeviceHint = "_foobar_"}
			};

			tableComponent.MappingConfig.Clear();
			tableComponent.MappingConfig.PopulateLamps(gameEngineLamps, tableComponent);

			tableComponent.MappingConfig.Lamps.Should().HaveCount(1);
			tableComponent.MappingConfig.Lamps[0].Id.Should().Be("11");
			tableComponent.MappingConfig.Lamps[0].Description.Should().Be("Foobar");
			tableComponent.MappingConfig.Lamps[0].Device.name.Should().Be("lamp_foobar_name");
			tableComponent.MappingConfig.Lamps[0].DeviceItem.Should().Be(LightComponent.LampIdDefault);
		}

		[Test]
		public void ShouldNotMapALampWithViaRegex()
		{
			var table = new TableBuilder()
				.AddLight("lamp_foobar_name")
				.Build();

			var go = VpxImportEngine.ImportIntoScene(table, options: ConvertOptions.SkipNone);
			var tableComponent = go.GetComponent<TableComponent>();

			var gameEngineLamps = new[] {
				new GamelogicEngineLamp("12") { Description = "Foobar", DeviceHint = "^_foobar_$"}
			};

			tableComponent.MappingConfig.Clear();
			tableComponent.MappingConfig.PopulateLamps(gameEngineLamps, tableComponent);

			tableComponent.MappingConfig.Lamps.Should().HaveCount(1);
			tableComponent.MappingConfig.Lamps[0].Id.Should().Be("12");
			tableComponent.MappingConfig.Lamps[0].Description.Should().Be("Foobar");
			tableComponent.MappingConfig.Lamps[0].Device.Should().BeNull();
			tableComponent.MappingConfig.Lamps[0].DeviceItem.Should().BeEmpty();
		}

		[Test]
		public void ShouldMapAnRgbLamp()
		{
			var table = new TableBuilder()
				.AddLight("my_rgb_light")
				.Build();

			var go = VpxImportEngine.ImportIntoScene(table, options: ConvertOptions.SkipNone);
			var tableComponent = go.GetComponent<TableComponent>();

			var gameEngineLamps = new[] {
				new GamelogicEngineLamp("r") { Description = "red", DeviceHint = "rgb", Channel = ColorChannel.Red},
				new GamelogicEngineLamp("g") { Description = "green", DeviceHint = "rgb", Channel = ColorChannel.Green},
				new GamelogicEngineLamp("b") { Description = "blue", DeviceHint = "rgb", Channel = ColorChannel.Blue},
			};

			tableComponent.MappingConfig.Clear();
			tableComponent.MappingConfig.PopulateLamps(gameEngineLamps, tableComponent);

			tableComponent.MappingConfig.Lamps.Should().HaveCount(3);
			tableComponent.MappingConfig.Lamps[2].Id.Should().Be("r");
			tableComponent.MappingConfig.Lamps[2].Channel.Should().Be(ColorChannel.Red);
			tableComponent.MappingConfig.Lamps[1].Id.Should().Be("g");
			tableComponent.MappingConfig.Lamps[1].Channel.Should().Be(ColorChannel.Green);
			tableComponent.MappingConfig.Lamps[0].Id.Should().Be("b");
			tableComponent.MappingConfig.Lamps[0].Channel.Should().Be(ColorChannel.Blue);

			tableComponent.MappingConfig.Lamps[0].Device.name.Should().Be("my_rgb_light");
			tableComponent.MappingConfig.Lamps[1].Device.name.Should().Be("my_rgb_light");
			tableComponent.MappingConfig.Lamps[2].Device.name.Should().Be("my_rgb_light");
			tableComponent.MappingConfig.Lamps[0].DeviceItem.Should().Be(LightComponent.LampIdDefault);
		}

		[Test]
		public void ShouldReturnAllLampIds()
		{
			var table = new TableBuilder()
				.AddLight("l11")
				.AddLight("l12")
				.Build();

			var go = VpxImportEngine.ImportIntoScene(table, options: ConvertOptions.SkipNone);
			var tableComponent = go.GetComponent<TableComponent>();

			var gameEngineLamps = new[] {
				new GamelogicEngineLamp("11")
			};

			tableComponent.MappingConfig.Clear();
			tableComponent.MappingConfig.PopulateLamps(gameEngineLamps, tableComponent);
			tableComponent.MappingConfig.AddLamp(new LampMapping {
				Id = "12",
				Device = go.transform.Find("Playfield/Lights/l12").GetComponent<LightComponent>(),
				DeviceItem = LightComponent.LampIdDefault
			});

			var lampIds = tableComponent.MappingConfig.GetLamps(gameEngineLamps).ToArray();

			lampIds.Length.Should().Be(2);
			lampIds[0].Id.Should().Be("11");
			lampIds[1].Id.Should().Be("12");
		}
	}
}
