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

using System;
using System.Collections.Generic;
using UnityEngine;
using VisualPinball.Engine.VPT;
using VisualPinball.Engine.VPT.Ramp;
using VisualPinball.Engine.VPT.Table;
using Mesh = VisualPinball.Engine.VPT.Mesh;

namespace VisualPinball.Unity
{
	[ExecuteInEditMode]
	[AddComponentMenu("Visual Pinball/Mesh/Ramp Floor Mesh")]
	public class RampFloorMeshComponent : MeshComponent<RampData, RampComponent>
	{
		public static readonly Type[] ValidParentTypes = Type.EmptyTypes;

		public override IEnumerable<Type> ValidParents => ValidParentTypes;

		protected override Mesh GetMesh(RampData _)
		{
			var playfieldComponent = GetComponentInParent<PlayfieldComponent>();
			return new RampMeshGenerator(MainComponent).GetMesh(playfieldComponent.Width, playfieldComponent.Height, playfieldComponent.PlayfieldHeight, RampMeshGenerator.Floor);
		}

		protected override PbrMaterial GetMaterial(RampData data, Table table)
			=> new RampMeshGenerator(MainComponent).GetMaterial(table, data);
	}
}
