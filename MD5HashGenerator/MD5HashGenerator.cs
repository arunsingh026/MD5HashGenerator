using System;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;

/// <summary>
/// Note: The file input should be in  "string 1|string 2|string 3|string 4" per line by line and after one line one space should be there if it's not, Please change the logic accordingly. 
/// Note: Output will be "string 1|string 2|string 3|<Hash value>|string 4" i.e: "3F206BD2D4FB8B6FBEF3E79E1F6C3964".
/// </summary>
namespace MD5HashGenerator
{
    public class MD5HashGenerator
    {
        private static readonly Object locker = new Object();

        static void Main(string[] args)
        {
            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            StreamReader file =
                new StreamReader(@"D:\TestFiles\ReadFrom.txt");
            while ((line = file.ReadLine()) != null)
            {
                // To ignore the white space in ODD numbers lines.
                if (counter % 2 == 0)
                {
                    var data = line.Split("|");
                    var hashkey = GenerateKey(data[2].ToString());
                    Console.WriteLine("The Hashkey of {0} is -> {1}", data[2].ToString(), hashkey);
                    // Write into file and download it.
                    GenerateFile(data, hashkey);
                }
                counter++;
            }
            file.Close();
            Console.ReadLine();
        }

        /// <summary>
        /// Generates a hashed - key for an instance of a class.
        /// The hash is a classic MD5 hash (e.g. BF20EB8D2C4901112179BF5D242D996B). So you can distinguish different 
        /// instances of a class. Because the object is hashed on the internal state, you can also hash it, then send it to
        /// someone in a serialized way. Your client can then deserialize it and check if it is in
        /// the same state.
        /// The method just just estimates that the object implements the ISerializable interface. What's
        /// needed to save the state or so, is up to the implementer of the interface.
        /// <b>The method is thread-safe!</b>
        /// </summary>
        /// <param name="sourceObject">The string you'd like to have a key out of it.</param>
        /// <returns>An string representing a MD5 Hashkey corresponding to the object or null if the object couldn't be serialized.</returns>
        /// <exception cref="ApplicationException">Will be thrown if the key cannot be generated.</exception>
        public static String GenerateKey(string sourceObject)
        {
            String hashString = "";

            //Catch unuseful parameter values
            if (sourceObject == null)
            {
                throw new ArgumentNullException("Null as parameter is not allowed");
            }
            else
            {
                //We determine if the passed object is really serializable.
                try
                {
                    //Now we begin to do the real work.
                    hashString = ComputeHash(ObjectToByteArray(sourceObject));
                    return hashString;
                }
                catch (AmbiguousMatchException ame)
                {
                    throw new ApplicationException("Could not definitly decide if object is serializable. Message:" + ame.Message);
                }
            }
        }

        /// <summary>
        /// Converts an object to an array of bytes. This array is used to hash the object.
        /// </summary>
        /// <param name="objectToSerialize">Just an object</param>
        /// <returns>A byte - array representation of the object.</returns>
        /// <exception cref="SerializationException">Is thrown if something went wrong during serialization.</exception>
        private static byte[] ObjectToByteArray(string objectToSerialize)
        {
            try
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(objectToSerialize);
                return inputBytes;
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Error occured during serialization. Message: " + se.Message);
                return null;
            }
        }

        /// <summary>
        /// Generates the hashcode of an given byte-array. The byte-array can be an object. Then the
        /// method "hashes" this object. The hash can then be used e.g. to identify the object.
        /// </summary>
        /// <param name="objectAsBytes">bytearray representation of an object.</param>
        /// <returns>The MD5 hash of the object as a string or null if it couldn't be generated.</returns>
        private static string ComputeHash(byte[] objectAsBytes)
        {
            MD5 md5 = MD5.Create();

            try
            {
                byte[] result = md5.ComputeHash(objectAsBytes);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch (ArgumentNullException ane)
            {
                //If something occured during serialization, this method is called with an null argument. 
                Console.WriteLine("Hash has not been generated.");
                return null;
            }
        }

        /// <summary>
        /// Add newly generated hash key into file line by line between two strings.
        /// </summary>
        /// <param name="data">Data representation string array.</param>
        /// <param name="hashkey">Hash key.</param>
        private static void GenerateFile(string[] data, string hashkey)
        {
            string value = "";
            string[] result = new string[5];
            result[0] = data[0];
            result[1] = data[1];
            result[2] = data[2];
            result[3] = hashkey;
            result[4] = data[3];
            for (int i = 0; i < result.Length; i++)
            {
                if (i != 0)
                {
                    value = value + "|" + result[i];
                }
                else
                {
                    value = result[i];
                }
            }
            TextWriter tw = File.AppendText(@"D:\TestFiles\WriteInto.txt");
            tw.WriteLine(value);
            tw.Close();
        }
    }
}
