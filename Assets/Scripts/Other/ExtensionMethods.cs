using UnityEngine;

namespace Other
{
    public static class ExtensionMethods
    {
        public static void Shuffle<T>(this T[] array)
        {
            var arrayLength = array.Length;
            while (arrayLength > 1)
            {
                int other = Random.Range(0, arrayLength--);
                (array[arrayLength], array[other]) = (array[other], array[arrayLength]);
            }
        }

        public static T TakeRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
    }
}