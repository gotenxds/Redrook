using System;
using UnityEngine.UI;

namespace UnityEngine
{
    public class FadeManager : MonoBehaviour
    {
        private static Image image;
        private static Animator animator;
        private void Start()
        {
            image = GetComponent<Image>();
            animator = GetComponent<Animator>();
            image.color = SetAlpha(image.color, 0);

        }

        public static void FadeOut()
        {
            image.color = SetAlpha(image.color, 1);
            image.enabled = true;
            
            animator.SetTrigger("fadeOut");
        }
        
        public static void FadeIn()
        {
            Debug.Log("HELLO");
            image.color = SetAlpha(image.color, 0);

            
            animator.SetTrigger("fadeIn");
        }


        private static Color SetAlpha(Color color, float alpha)
        {
            color.a = alpha;

            return color;
        }
    }
}