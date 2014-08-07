namespace SharpBattleNet.Framework.Utilities.Debugging
{
    using System;

    public static class Guard
    {
        public static void AgainstNull(object valueToTest)
        {
            if (null == valueToTest)
            {
                throw new ArgumentNullException("", "The parameter cannot be null");
            }

            return;
        }

        public static void AgainstEmptyString(string valueToTest)
        {
            if (string.IsNullOrEmpty(valueToTest))
            {
                throw new ArgumentException("The passed in string cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(valueToTest))
            {
                throw new ArgumentException("The passed in string is empty or contains only white-spaces");
            }

            return;
        }
    }
}
