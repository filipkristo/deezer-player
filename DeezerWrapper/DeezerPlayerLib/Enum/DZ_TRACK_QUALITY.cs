using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeezerPlayerLib.Enum
{
    public enum DZ_TRACK_QUALITY
    {
        DZ_TRACK_QUALITY_UNKNOWN,       /**< Track quality has not been set yet, not a valid value. */
        DZ_TRACK_QUALITY_STANDARD,      /**< Medium quality compressed audio.     */
        DZ_TRACK_QUALITY_HIGHQUALITY,   /**< High quality compressed audio.       */
        DZ_TRACK_QUALITY_CDQUALITY,     /**< Lossless two channel 44,1KHz 16bits. */
        DZ_TRACK_QUALITY_DATA_EFFICIENT,/**< Try using smaller file formats.      */

        DZ_TRACK_QUALITY_LAST_ENTRY     /**< NOT a valid value, just for internal purpose of array sizing, keep it at the last position */
    }
}
