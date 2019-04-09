using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileSets.Brushes.Tint_Brush_Smooth.Scripts.Editor
{
	[CustomGridBrush(false, false, false, "Tint Brush (Smooth)")]
	public class TintBrushSmooth : GridBrushBase
	{
		[SerializeField] private float mBlend = 1f;
		[SerializeField] private Color mColor = Color.white;

		public float blend
		{
			get { return mBlend; }
			set { mBlend = value; }
		}

		public Color color
		{
			get { return mColor; }
			set { mColor = value; }
		}

		private static bool IsInBounds(GridLayout grid, Vector3 position)
		{
			if (GetGenerator(grid).IsValidPosition(position))
			{
				return true;
			}

			EditorUtility.DisplayDialog("Ooops, Anna fucked up", "You are trying to draw out of bounds, wrong tilemap selected?", "Sorry...");
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
			
			var generator = GetGenerator(grid);

			if (generator == null)
			{
				return;
			}

			var oldColor = generator.GetColor(grid as Grid, position);
			var blendColor = oldColor * (1 - mBlend) + mColor * mBlend;
			generator.SetColor(grid as Grid, position, blendColor);
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
			
			var generator = GetGenerator(grid);
			if (generator != null)
			{
				mColor = generator.GetColor(grid as Grid, position.min);
			}
		}

		private static TintTextureGenerator GetGenerator(GridLayout grid)
		{
			var generator = FindObjectOfType<TintTextureGenerator>();
			if (generator != null)
			{
				return generator;
			}
			
			if (grid != null)
			{
				generator = grid.gameObject.AddComponent<TintTextureGenerator>();
			}

			return generator;
		}
	}

	[CustomEditor(typeof(TintBrushSmooth))]
	public class TintBrushSmoothEditor : GridBrushEditorBase
	{
		private TintBrushSmooth brush { get { return target as TintBrushSmooth; } }

		public override GameObject[] validTargets
		{
			get
			{
				return FindObjectsOfType<Tilemap>().Select(x => x.gameObject).ToArray();
			}
		}

		public override void OnPaintInspectorGUI()
		{
			brush.color = EditorGUILayout.ColorField("Color", brush.color);
			brush.blend = EditorGUILayout.Slider("Blend", brush.blend, 0f, 1f);
			GUILayout.Label("Note: Tilemap needs to use TintedTilemap.shader!");
		}
	}
}
