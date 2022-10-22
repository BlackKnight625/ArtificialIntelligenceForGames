namespace Assets.Scripts.IAJ.Unity.Utils {
    public static class Utils {
        public static T[] CreateCopy<T>(this T[] array) {
            int length = array.Length;
            T[] newArray = new T[length];

            for (int i = 0; i < length; i++) {
                newArray[i] = array[i];
            }

            return newArray;
        }
    }
}