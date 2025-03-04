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

#region ReSharper
// ReSharper disable UnassignedField.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using VisualPinball.Engine.IO;
using VisualPinball.Engine.Math;
using VisualPinball.Engine.VPT.Table;

namespace VisualPinball.Engine.VPT.Trigger
{
	[Serializable]
	public class TriggerData : ItemData
	{
		public override string GetName() => Name;
		public override void SetName(string name) { Name = name; }

		[BiffString("NAME", IsWideString = true, Pos = 14)]
		public string Name = string.Empty;

		[BiffDragPoint("DPNT", TagAll = true, Pos = 2000)]
		public DragPointData[] DragPoints;

		[BiffVertex("VCEN", Pos = 1)]
		public Vertex2D Center;

		[BiffFloat("RADI", Pos = 2)]
		public float Radius = 25f;

		[BiffFloat("ROTA", Pos = 3)]
		public float Rotation = 0f;

		[BiffFloat("SCAX", Pos = 5)]
		public float ScaleX = 1f;

		[BiffFloat("SCAY", Pos = 6)]
		public float ScaleY = 1f;

		[BiffString("MATR", Pos = 10)]
		public string Material = string.Empty;

		[BiffString("SURF", Pos = 9)]
		public string Surface = string.Empty;

		[BiffBool("VSBL", Pos = 12)]
		public bool IsVisible = true;

		[BiffBool("EBLD", Pos = 11)]
		public bool IsEnabled = true;

		[BiffFloat("THOT", Pos = 13)]
		public float HitHeight = 50f;

		[BiffInt("SHAP", Pos = 15)]
		public int Shape = TriggerShape.TriggerWireA;

		[BiffFloat("ANSP", Pos = 16)]
		public float AnimSpeed = 1f;

		[BiffFloat("WITI", Pos = 4)]
		public float WireThickness = 0f;

		[BiffBool("REEN", Pos = 17)]
		public bool IsReflectionEnabled = true;

		[BiffBool("TMON", Pos = 7)]
		public bool IsTimerEnabled;

		[BiffInt("TMIN", Pos = 8)]
		public int TimerInterval;

		public TriggerData() : base(StoragePrefix.GameItem)
		{
		}

		public TriggerData(string name, float x, float y) : base(StoragePrefix.GameItem)
		{
			Name = name;
			Center = new Vertex2D(x, y);
		}

		#region BIFF

		static TriggerData()
		{
			Init(typeof(TriggerData), Attributes);
		}

		public TriggerData(BinaryReader reader, string storageName) : base(storageName)
		{
			Load(this, reader, Attributes);
		}

		public override void Write(BinaryWriter writer, HashWriter hashWriter)
		{
			writer.Write((int)ItemType.Trigger);
			WriteRecord(writer, Attributes, hashWriter);
			WriteEnd(writer, hashWriter);
		}

		private static readonly Dictionary<string, List<BiffAttribute>> Attributes = new Dictionary<string, List<BiffAttribute>>();

		#endregion
	}
}
