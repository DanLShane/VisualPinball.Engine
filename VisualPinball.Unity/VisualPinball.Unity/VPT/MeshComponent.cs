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
using UnityEngine;
using VisualPinball.Engine.VPT;
using VisualPinball.Engine.VPT.Table;
using Mesh = VisualPinball.Engine.VPT.Mesh;

namespace VisualPinball.Unity
{
	public abstract class MeshComponent<TData, TComponent> : SubComponent<TData, TComponent>,
		IMeshComponent
		where TData : ItemData
		where TComponent : MainRenderableComponent<TData>
	{
		public IMainRenderableComponent MainRenderableComponent => MainComponent;

		#region Creation and destruction

		private void OnEnable()
		{
			var mr = gameObject.GetComponent<MeshRenderer>();
			if (mr != null) {
				mr.enabled = true;
			}
		}

		private void OnDisable()
		{
			var mr = gameObject.GetComponent<MeshRenderer>();
			if (mr != null) {
				mr.enabled = false;
			}
		}

		#endregion

		public void RebuildMeshes()
		{
			UpdateMesh();
		}

		public void ClearMeshVertices()
		{
			var mf = GetComponent<MeshFilter>();
			if (mf && mf.sharedMesh) {
				var mesh = mf.sharedMesh;
				mesh.triangles = null;
				mesh.vertices = Array.Empty<Vector3>();
				mesh.normals = Array.Empty<Vector3>();
				mesh.uv = Array.Empty<Vector2>();
			}
		}

		protected abstract Mesh GetMesh(TData data);

		protected abstract PbrMaterial GetMaterial(TData data, Table table);

		public void CreateMesh(TData data, Table table, ITextureProvider texProvider, IMaterialProvider matProvider)
		{
			CreateMesh(gameObject, GetMesh(data), GetMaterial(data, table), data.GetName(), texProvider, matProvider);
		}

		public static void CreateMesh(GameObject gameObject, Mesh m, PbrMaterial material, string name, ITextureProvider texProvider, IMaterialProvider matProvider)
		{
			if (m == null) {
				return;
			}
			var mesh = m.ToUnityMesh($"{name} Mesh ({gameObject.name})");

			// apply mesh to game object
			var mf = gameObject.GetComponent<MeshFilter>();
			mf.sharedMesh = mesh;

			// apply renderer and material
			if (m.AnimationFrames.Count > 0) { // if number of animations frames are 1, the blend vertices are in the uvs are handle by the lerp shader.
				var smr = gameObject.AddComponent<SkinnedMeshRenderer>();
				smr.sharedMaterial = material.ToUnityMaterial(matProvider, texProvider);
				smr.sharedMesh = mesh;
				smr.SetBlendShapeWeight(0, m.AnimationDefaultPosition);
			} else {
				var mr = gameObject.AddComponent<MeshRenderer>();
				mr.sharedMaterial = material.ToUnityMaterial(matProvider, texProvider);
			}
		}

		private void UpdateMesh()
		{
			var data = MainComponent.InstantiateData();
			MainComponent.CopyDataTo(data, null, null, false);
			var mesh = GetMesh(data);

			// mesh generator can return null - but in this case the main component
			// will take care of removing the mesh component.
			if (mesh == null || !mesh.IsSet) {
				return;
			}
			var mf = GetComponent<MeshFilter>();

			if (mf != null) {
				var unityMesh = mf.sharedMesh;
				if (!unityMesh) {
					mf.sharedMesh = new UnityEngine.Mesh() { name = $"{name} (Generated)" };
					unityMesh = mf.sharedMesh;
				}
				mesh.ApplyToUnityMesh(unityMesh);
			}
		}
	}
}
