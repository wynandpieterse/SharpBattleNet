#region Header
//
//    _  _   ____        _   _   _         _   _      _   
//  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_ 
// |_  .. _ |  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __|
// |_      _| |_) | (_| | |_| |_| |  __/_ | |\  |  __/ |_ 
//   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__|
//
// The MIT License
// 
// Copyright(c) 2014 Wynand Pieters. https://github.com/wpieterse/SharpBattleNet

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

namespace SharpBattleNet.Runtime.Utilities.Debugging
{
    #region Usings
    using System;
    #endregion
    
    /// <summary>
    /// Usefull methods to test class pre- and post-conditions.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Guards agains a value that is null.
        /// </summary>
        /// <param name="valueToTest">
        /// The value to test.
        /// </param>
        public static void AgainstNull(object valueToTest)
        {
            if (null == valueToTest)
            {
                throw new ArgumentNullException("", "The parameter cannot be null");
            }

            return;
        }

        /// <summary>
        /// Guards agains null, empty and whitespace strings.
        /// </summary>
        /// <param name="valueToTest">
        /// The string to test.
        /// </param>
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

        /// <summary>
        /// Guards agains an object that is disposed.
        /// </summary>
        /// <param name="disposed">Wheter the class is disposed or not.</param>
        public static void AgainstDispose(bool disposed)
        {
            if(true == disposed)
            {
                throw new ObjectDisposedException("Object has been disposed. Cannot use it anymore.");
            }

            return;
        }
    }
}
