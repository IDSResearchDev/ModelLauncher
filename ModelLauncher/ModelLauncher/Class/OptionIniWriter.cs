using Rnd.Common;

namespace ModelLauncher.Class
{
    /// <summary>
    /// Manipulates tekla options.ini 
    /// </summary>
    public class OptionIniWriter
    {   
        /// <summary>
        /// Method that writes on tekla options.ini.
        /// </summary>
        /// <param name="modelPathOptionsIni">Gets the path of a specific folder of options.ini.</param>
        /// <param name="modelFirmPath">Gets the path of specified model in firm folder.</param>
        public static void WriteOptionsIni(string modelPathOptionsIni, string modelFirmPath)
        {
            string filename = Utilities.PathFilename(modelPathOptionsIni, "options",".ini");


            //Read XML with return string x
            //Wrte xml string x
            //var checkbool = Utilities.CheckIfFileExists(filename);
            if (!Utilities.CheckIfFileExists(filename))
            {
                Utilities.CreateFile(modelPathOptionsIni,"options",".ini");
                //CreateFile(ModelPathOptionsIni,"options",".ini");
                Utilities.ReadWriteLines(filename, "XS_FIRM=" + modelFirmPath, "XS_FIRM=");
            }
            else
            {
                Utilities.ReadWriteLines(filename, "XS_FIRM=" + modelFirmPath, "XS_FIRM=");
            }
        }
    }
}
