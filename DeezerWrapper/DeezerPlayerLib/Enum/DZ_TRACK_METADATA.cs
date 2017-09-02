using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeezerPlayerLib.Enum
{
    public enum DZ_TRACK_METADATA
    {
        DZ_TRACK_METADATA_UNKNOWN,       /**< Track metadata has not been set yet, not a valid value. */
        DZ_TRACK_METADATA_FORMAT_HEADER, /**< Track header metadata type. */
        DZ_TRACK_METADATA_DURATION_MS,   /**< Track duration metadata type. */
    }
}
