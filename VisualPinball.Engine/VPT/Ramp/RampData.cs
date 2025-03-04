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
// ReSharper disable InconsistentNaming
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using VisualPinball.Engine.IO;
using VisualPinball.Engine.Math;
using VisualPinball.Engine.VPT.Table;

namespace VisualPinball.Engine.VPT.Ramp
{
	[Serializable]
	public class RampData : ItemData, IRampData
	{
		public override string GetName() => Name;
		public override void SetName(string name) { Name = name; }

		[BiffString("NAME", IsWideString = true, Pos = 9)]
		public string Name = string.Empty;

		[BiffFloat("RADB", Pos = 24)]
		public float DepthBias = 0f;

		[BiffDragPoint("DPNT", TagAll = true, Pos = 2000)]
		public DragPointData[] DragPoints { get; set; }

		[BiffFloat("ELAS", Pos = 19)]
		public float Elasticity;

		[BiffFloat("RFCT", Pos = 20)]
		public float Friction;

		[BiffBool("HTEV", Pos = 17)]
		public bool HitEvent = false;

		[BiffFloat("HTBT", Pos = 1)]
		public float HeightBottom { get; set; }  = 0f;

		[BiffFloat("HTTP", Pos = 2)]
		public float HeightTop { get; set; }  = 50f;

		[BiffInt("ALGN", Pos = 11)]
		public int ImageAlignment { get; set; } = RampImageAlignment.ImageModeWorld;

		[BiffBool("IMGW", Pos = 12)]
		public bool ImageWalls = true;

		[BiffBool("CLDR", Pos = 22)]
		public bool IsCollidable = true;

		[BiffBool("REEN", Pos = 28)]
		public bool IsReflectionEnabled = true;

		[BiffBool("RVIS", Pos = 23)]
		public bool IsVisible = true;

		[BiffFloat("WLHL", Pos = 13)]
		public float LeftWallHeight = 62f;

		[BiffFloat("WVHL", Pos = 15)]
		public float LeftWallHeightVisible { get; set; } = 30f;

		[BiffBool("OVPH", Pos = 30)]
		public bool OverwritePhysics = true;

		[BiffInt("TYPE", Pos = 8)]
		public int RampType = VisualPinball.Engine.VPT.RampType.RampTypeFlat;

		[BiffFloat("WLHR", Pos = 14)]
		public float RightWallHeight { get; set; } = 62f;

		[BiffFloat("WVHR", Pos = 16)]
		public float RightWallHeightVisible { get; set; } = 30f;

		[BiffFloat("RSCT", Pos = 21)]
		public float Scatter;

		[BiffString("IMAG", Pos = 10)]
		public string Image = string.Empty;

		[BiffString("MATR", Pos = 5)]
		public string Material = string.Empty;

		[BiffString("MAPH", Pos = 29)]
		public string PhysicsMaterial = string.Empty;

		[BiffFloat("THRS", Pos = 18)]
		public float Threshold;

		[BiffFloat("WDBT", Pos = 3)]
		public float WidthBottom { get; set; } = 75f;

		[BiffFloat("WDTP", Pos = 4)]
		public float WidthTop { get; set; } = 60f;

		[BiffFloat("RADI", Pos = 25)]
		public float WireDiameter { get; set; } = 8f;

		[BiffFloat("RADX", Pos = 26)]
		public float WireDistanceX { get; set; } = 38f;

		[BiffFloat("RADY", Pos = 27)]
		public float WireDistanceY { get; set; } = 88f;

		[BiffBool("TMON", Pos = 6)]
		public bool IsTimerEnabled;

		[BiffInt("TMIN", Pos = 7)]
		public int TimerInterval;

		[BiffTag("PNTS", Pos = 1999)]
		public bool Points;

		public int Type => RampType;

		public RampData() : base(StoragePrefix.GameItem)
		{
		}

		public RampData(string name, DragPointData[] dragPoints) : base(StoragePrefix.GameItem)
		{
			Name = name;
			DragPoints = dragPoints;
		}

		#region BIFF

		static RampData()
		{
			Init(typeof(RampData), Attributes);
		}

		public RampData(BinaryReader reader, string storageName) : base(storageName)
		{
			Load(this, reader, Attributes);
		}

		public override void Write(BinaryWriter writer, HashWriter hashWriter)
		{
			writer.Write((int)ItemType.Ramp);
			WriteRecord(writer, Attributes, hashWriter);
			WriteEnd(writer, hashWriter);
		}

		private static readonly Dictionary<string, List<BiffAttribute>> Attributes = new Dictionary<string, List<BiffAttribute>>();

		#endregion
	}
}
