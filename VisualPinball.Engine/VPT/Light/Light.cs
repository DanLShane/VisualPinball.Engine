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
using System.IO;
using VisualPinball.Engine.Game;
using VisualPinball.Engine.Math;

namespace VisualPinball.Engine.VPT.Light
{
	public class Light : Item<LightData>, IRenderable
	{
		public override string ItemGroupName => "Lights";

		public const string BulbMaterialName = "__bulbMaterial";
		public const string SocketMaterialName = "__bulbSocketMaterial";

		private readonly LightMeshGenerator _meshGenerator;

		public bool IsInsertLight(Table.Table table) => !Data.ShowBulbMesh && string.Equals(Data.OffImage, table.Data.Image, StringComparison.OrdinalIgnoreCase);

		public Light(LightData data) : base(data)
		{
			_meshGenerator = new LightMeshGenerator(Data);
		}

		public Light(BinaryReader reader, string itemName) : this(new LightData(reader, itemName))
		{
		}

		public static Light GetDefault(string name, float x, float y)
		{
			var lightData = new LightData(name, x, y) {
				DragPoints = new[] {
					new DragPointData(x, y - 50f) {IsSmooth = true },
					new DragPointData(x - 50f * MathF.Cos(MathF.PI / 4), y - 50f * MathF.Sin(MathF.PI / 4)) {IsSmooth = true },
					new DragPointData(x - 50f, y) {IsSmooth = true },
					new DragPointData(x - 50f * MathF.Cos(MathF.PI / 4), y + 50f * MathF.Sin(MathF.PI / 4)) {IsSmooth = true },
					new DragPointData(x, y + 50f) {IsSmooth = true },
					new DragPointData(x + 50f * MathF.Cos(MathF.PI / 4), y + 50f * MathF.Sin(MathF.PI / 4)) {IsSmooth = true },
					new DragPointData(x + 50f, y) {IsSmooth = true },
					new DragPointData(x + 50f * MathF.Cos(MathF.PI / 4), y - 50f * MathF.Sin(MathF.PI / 4)) {IsSmooth = true },
				}
			};
			return new Light(lightData);
		}
		
		public static Light GetDefault(Table.Table table) => GetDefault(table.GetNewName<Light>("Light"), table.Width / 2f, table.Height / 2f);

		#region IRenderable

		Matrix3D IRenderable.TransformationMatrix(Table.Table table, Origin origin)
			=> _meshGenerator.GetPostMatrix(table, origin);

		public Mesh GetMesh(string id, Table.Table table, Origin origin = Origin.Global, bool asRightHanded = true)
			=> _meshGenerator.GetMesh(id, table, origin, asRightHanded);

		public PbrMaterial GetMaterial(string id, Table.Table table) => _meshGenerator.GetMaterial(id, table);

		#endregion
	}
}
