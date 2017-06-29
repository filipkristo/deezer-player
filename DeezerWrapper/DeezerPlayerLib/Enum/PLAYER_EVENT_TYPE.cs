// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Enum
{
    public enum PLAYER_EVENT_TYPE
    {
        DZ_PLAYER_EVENT_UNKNOWN,                             /**< Player event has not been set yet, not a valid value. */

        // Data access related event.
        DZ_PLAYER_EVENT_LIMITATION_FORCED_PAUSE,             /**< Another deezer player session was created elsewhere, the player has entered pause mode. */

        // Track selection related event.
        DZ_PLAYER_EVENT_QUEUELIST_LOADED,                    /**< Content has been loaded. */
        DZ_PLAYER_EVENT_QUEUELIST_NO_RIGHT,                  /**< You don't have the right to play this content: track, album or playlist */
        DZ_PLAYER_EVENT_QUEUELIST_TRACK_NOT_AVAILABLE_OFFLINE,/**< You're offline, the track is not available. */
        DZ_PLAYER_EVENT_QUEUELIST_TRACK_RIGHTS_AFTER_AUDIOADS,/**< You have right to play it, but you should render an ads first :
                                                              - Use dz_player_event_get_advertisement_infos_json().
                                                              - Play an ad with dz_player_play_audioads().
                                                              - Wait for #DZ_PLAYER_EVENT_RENDER_TRACK_END.
                                                              - Use dz_player_play() with previous track or DZ_PLAYER_PLAY_CMD_RESUMED_AFTER_ADS (to be done even on mixes for now).
                                                          */
        DZ_PLAYER_EVENT_QUEUELIST_SKIP_NO_RIGHT,              /**< You're on a mix, and you had no right to do skip. */

        DZ_PLAYER_EVENT_QUEUELIST_TRACK_SELECTED,             /**< A track is selected among the ones available on the server, and will be fetched and read. */

        DZ_PLAYER_EVENT_QUEUELIST_NEED_NATURAL_NEXT,          /**< We need a new natural_next action. */

        // Data loading related event.
        DZ_PLAYER_EVENT_MEDIASTREAM_DATA_READY,              /**< Data is ready to be introduced into audio output (first data after a play). */
        DZ_PLAYER_EVENT_MEDIASTREAM_DATA_READY_AFTER_SEEK,   /**< Data is ready to be introduced into audio output (first data after a seek). */

        // Play (audio rendering on output) related event.
        DZ_PLAYER_EVENT_RENDER_TRACK_START_FAILURE,       /**< Error, track is unable to play. */
        DZ_PLAYER_EVENT_RENDER_TRACK_START,               /**< A track has started to play. */
        DZ_PLAYER_EVENT_RENDER_TRACK_END,                 /**< A track has stopped because the stream has ended. */
        DZ_PLAYER_EVENT_RENDER_TRACK_PAUSED,              /**< Currently on paused. */
        DZ_PLAYER_EVENT_RENDER_TRACK_SEEKING,             /**< Waiting for new data on seek. */
        DZ_PLAYER_EVENT_RENDER_TRACK_UNDERFLOW,           /**< Underflow happened whilst playing a track. */
        DZ_PLAYER_EVENT_RENDER_TRACK_RESUMED,             /**< Player resumed play after a underflow or a pause. */
        DZ_PLAYER_EVENT_RENDER_TRACK_REMOVED,             /**< Player stopped playing a track. */
    };
}
