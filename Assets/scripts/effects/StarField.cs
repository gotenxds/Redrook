using UnityEngine;
using UnityEngine.Assertions;

namespace effects
{
	public class StarField : MonoBehaviour
	{
		[SerializeField] private int maxStars = 100;
		[SerializeField] private float starSize = 0.1f;
		[SerializeField] private float starSizeRange = 0.5f;
		[SerializeField] private float fieldWidth = 20f;
		[SerializeField] private float fieldHeight = 25f;
		[SerializeField] private float parallexModifier = 0f;
		[SerializeField] private Color32 color;
		
		private ParticleSystem Particles;
		private ParticleSystem.Particle[] Stars;
		private new Transform camera;
		private Vector3 cameraLastPosition;
		private float xOffset;
		private float yOffset;

		private void Awake ()
		{
			if (Camera.main != null) camera = Camera.main.transform;
			cameraLastPosition = camera.position;

			Stars = new ParticleSystem.Particle[ maxStars ];
			Particles = GetComponent<ParticleSystem>();
			Assert.IsNotNull(Particles, "Particle system missing from object!" );

			xOffset = fieldWidth * 0.5f;																										// Offset the coordinates to distribute the spread
			yOffset = fieldHeight * 0.5f;																										// around the object's center

			for( int i=0; i<maxStars; i++ )
			{
				float randSize = Random.Range( starSizeRange, starSizeRange + 1f );						// Randomize star size within parameters
 
				Stars[i].position = GetRandomInRectangle( fieldWidth, fieldHeight ) + transform.position;
				Stars[i].startSize = starSize * randSize;
				Stars[i].startColor = color;
			}
			Particles.SetParticles( Stars, Stars.Length );  																// Write data to the particle system
		}

		private Vector3 GetRandomInRectangle ( float width, float height )
		{
			var x = Random.Range( 0, width );
			var y = Random.Range( 0, height );
			return new Vector3 ( x - xOffset , y - yOffset, 0 );
		}

		private void Update()
		{
			var position = camera.position;
			var delta = cameraLastPosition - position;
			
			cameraLastPosition = position;

			transform.position += delta * parallexModifier;
		}
	}
}