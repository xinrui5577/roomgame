#if !UNITY_FLASH
using System.IO;

namespace Assets.Scripts.Game.lyzz2d.Game.ImgPress
{ //Copy from http://mcain.bluehoststaff.com/gamedesignftp/backup_Corey/Mega-Fiers/Scripts/MegaGrab/JPGEncoder.cs

    public class ByteArrayTool
    {
        private readonly MemoryStream stream;
        private readonly BinaryWriter writer;

        public ByteArrayTool()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        /**
    * Function from AS3--add a byte to our stream
    */

        public void writeByte(byte value)
        {
            writer.Write(value);
        }

        /**
    * Spit back all bytes--to either pass via WWW or save to disk
    */

        public byte[] GetAllBytes()
        {
            var buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }
    }

/**
* This should really be a struct--if you care, declare it in C#
*/

/**
* Another flash class--emulating the stuff the encoder uses
*/

/**
 * Class that converts BitmapData into a valid JPEG
 */

/*
* Ported to UnityScript by Matthew Wegner, Flashbang Studios
* 
* Original code is from as3corelib, found here:
* http://code.google.com/p/as3corelib/source/browse/trunk/src/com/adobe/images/JPGEncoder.as
* 
* Original copyright notice is below:
*/

/*
* Ported to C# by Tony McBride
*
* C# version isnt threaded so just call like this:
* JPGEncoder NewEncoder = new JPGEncoder( MyTexture , 75.0f );
* NewEncoder.doEncoding();
* byte[] TexData = NewEncoder.GetBytes();
*/
#endif
}