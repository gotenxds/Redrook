using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
	[CustomGridBrush(false, false, false, "Tint Brush (Smooth)")]
	public class TintBrushSmooth : GridBrushBase
	{
		public float m_Blend = 1f;
		public Color m_Color = Color.white;

		private bool IsInBounds(GridLayout grid, Vector3 position)
		{
			if (GetGenerator(grid).IsValidPosition(position))
			{
				return true;
			}

			EditorUtility.DisplayDialog("Ooops, anna fucked up", "You are trying to draw out of bounds, wrong tilemap selected?", "Sorry...");
			return false;
		}
		
		public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

			if (!IsInBounds(grid, position))
			{
				return;
			}
			TintTextureGenerator generator = GetGenerator(grid);
			if (generator != null)
			{
				var oldColor = generator.GetColor(grid as Grid, position);
				var blendColor = oldColor * (1 - m_Blend) + m_Color * m_Blend;
				generator.SetColor(grid as Grid, position, blendColor);
			}
		}

		public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			Debug.Log(position);
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
			{
				return;
			}

			if (!IsInBounds(grid, position))
			{
				return;
			}

			var generator = GetGenerator(grid);
			if (generator != null)
			{
				generator.SetColor(grid as Grid, position, Color.white);
			}
		}

		public override void Pick(GridLayout grid, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

			if (!IsInBounds(grid, position.center))
			{
				return;
			}
			
			TintTextureGenerator generator = GetGenerator(grid);
			if (generator != null)
			{
				m_Color = generator.GetColor(grid as Grid, position.min);
			}
		}

		private static TintTextureGenerator GetGenerator(GridLayout grid)
		{
			TintTextureGenerator generator = FindObjectOfType<TintTextureGenerator>();
			if (generator == null)
			{
				if (grid != null)
				{
					generator = grid.gameObject.AddComponent<TintTextureGenerator>();
				}
			}
			return generator;
		}
	}

	[CustomEditor(typeof(TintBrushSmooth))]
	public class TintBrushSmoothEditor : GridBrushEditorBase
	{
		public TintBrushSmooth brush { get { return target as TintBrushSmooth; } }

		public override GameObject[] validTargets
		{
			get
			{
				return FindObjectsOfType<Tilemap>().Select(x => x.gameObject).ToArray();
			}
		}

		public override void OnPaintInspectorGUI()
		{
			brush.m_Color = EditorGUILayout.ColorField("Color", brush.m_Color);
			brush.m_Blend = EditorGUILayout.Slider("Blend", brush.m_Blend, 0f, 1f);
			GUILayout.Label("Note: Tilemap needs to use TintedTilemap.shader!");
		}
	}
}
