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

// ReSharper disable AssignmentInConditionalExpression

using UnityEditor;
using VisualPinball.Engine.VPT.Primitive;

namespace VisualPinball.Unity.Editor
{
	[CustomEditor(typeof(PrimitiveComponent)), CanEditMultipleObjects]
	public class PrimitiveInspector : MainInspector<PrimitiveData, PrimitiveComponent>
	{
		private SerializedProperty _positionProperty;
		private SerializedProperty _rotationProperty;
		private SerializedProperty _sizeProperty;
		private SerializedProperty _translationProperty;
		private SerializedProperty _objectRotationProperty;

		protected override void OnEnable()
		{
			base.OnEnable();

			_positionProperty = serializedObject.FindProperty(nameof(PrimitiveComponent.Position));
			_rotationProperty = serializedObject.FindProperty(nameof(PrimitiveComponent.Rotation));
			_sizeProperty = serializedObject.FindProperty(nameof(PrimitiveComponent.Size));
			_translationProperty = serializedObject.FindProperty(nameof(PrimitiveComponent.Translation));
			_objectRotationProperty = serializedObject.FindProperty(nameof(PrimitiveComponent.ObjectRotation));
		}

		public override void OnInspectorGUI()
		{
			if (HasErrors()) {
				return;
			}

			BeginEditing();

			OnPreInspectorGUI();

			PropertyField(_positionProperty, updateTransforms: true);
			PropertyField(_rotationProperty, updateTransforms: true);
			PropertyField(_sizeProperty, updateTransforms: true);
			PropertyField(_translationProperty, updateTransforms: true);
			PropertyField(_objectRotationProperty, updateTransforms: true);

			base.OnInspectorGUI();

			EndEditing();
		}
	}
}
