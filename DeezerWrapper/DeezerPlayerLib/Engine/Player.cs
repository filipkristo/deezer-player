using DeezerPlayer.Model;
using DeezerPlayerLib.Enum;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
// make this binding dependent on WPF, but easier to use

// http://www.codeproject.com/Articles/339290/PInvoke-pointer-safety-Replacing-IntPtr-with-unsaf

namespace DeezerPlayerLib.Engine
{
    unsafe public class Player : IDisposable
    {
        static Hashtable refKeeper = new Hashtable();

        internal unsafe PLAYER* libcPlayerHndl;
        internal Connect connect;

        internal libcPlayerOnEventCb eventcb;
        internal libcPlayerOnMetaDataCb onMetaDatacb;
        internal libcPlayerOnIndexProgressCb onIndexProgress;
        internal libcPlayerOnRenderProgressCb onRenderProgress;

        public event EventHandler<Song> SongChanged;
        public event EventHandler<ulong> ProgressChanged;
        public event EventHandler<ulong> RenderChanged;

        private const int DZ_INDEX_IN_QUEUELIST_INVALID = -1;
        private const int DZ_INDEX_IN_QUEUELIST_PREVIOUS = -2;
        private const int DZ_INDEX_IN_QUEUELIST_CURRENT = -3;
        private const int DZ_INDEX_IN_QUEUELIST_NEXT = -4;

        public bool IsPlaying { get; private set; }
        public string CurrentStation { get; private set; }
        public Song CurrentSong { get; private set; }

        public unsafe Player(Connect connect, object observer)
        {
            IntPtr intptr = new IntPtr(this.GetHashCode());
            refKeeper[intptr] = this;

            libcPlayerHndl = dz_player_new(connect.libcConnectHndl);
            this.connect = connect;

            onMetaDatacb = (libcPlayer, trackMetadata, userdata) =>
            {
                Debug.WriteLine($"trackMetadata: {trackMetadata}");
            };
            dz_player_set_metadata_cb(libcPlayerHndl, onMetaDatacb);

            onIndexProgress = (libcPlayer, progressMicroseconds, userdata) =>
            {
                OnProgressChanged(progressMicroseconds);
            };
            dz_player_set_index_progress_cb(libcPlayerHndl, onIndexProgress, 1000000);

            onRenderProgress = (libcPlayer, progressMicroseconds, userdata) =>
            {
                OnRenderChanged(progressMicroseconds);
            };
            dz_player_set_render_progress_cb(libcPlayerHndl, onRenderProgress, 1000000);
        }

        public ERRORS Start(PlayerOnEventCb eventcb)
        {
            ERRORS ret;
            ret = dz_player_activate(libcPlayerHndl, new IntPtr(this.GetHashCode()));
            Console.WriteLine($"dz_player_activate result: {ret}");

            this.eventcb = delegate (PLAYER* libcPlayer, PLAYER_EVENT* libcPlayerEvent, IntPtr userdata)
            {
                Player player = (Player)refKeeper[userdata];
                PlayerEvent playerEvent = PlayerEvent.newFromLibcEvent(libcPlayerEvent);

                STREAMING_MODE streamingMode;
                int idx;

                var result = dz_player_event_get_queuelist_context(libcPlayerEvent, &streamingMode, &idx);

                if (!result)
                {
                    streamingMode = STREAMING_MODE.DZ_STREAMING_MODE_ONDEMAND;
                    idx = DZ_INDEX_IN_QUEUELIST_INVALID;
                }

                if (playerEvent.eventType == PLAYER_EVENT_TYPE.DZ_PLAYER_EVENT_QUEUELIST_TRACK_SELECTED)
                {
                    bool isPreview;
                    bool canPauseUnPause;
                    bool canSeek;
                    int numberSkipAllowed;

                    isPreview = dz_player_event_track_selected_is_preview(libcPlayerEvent);
                    var ok = dz_player_event_track_selected_rights(libcPlayerEvent, &canPauseUnPause, &canSeek, &numberSkipAllowed);

                    var songIntPtr = dz_player_event_track_selected_dzapiinfo(libcPlayerEvent);
                    var nextIntPtr = dz_player_event_track_selected_next_track_dzapiinfo(libcPlayerEvent);

                    var currentSongStr = NativeMethods.StringFromNativeUtf8(songIntPtr);
                    var nextSongStr = NativeMethods.StringFromNativeUtf8(nextIntPtr);

                    CurrentSong = JsonConvert.DeserializeObject<Song>(currentSongStr);

                    OnSongChanged(CurrentSong);
                    IsPlaying = true;
                }
                if (playerEvent.eventType == PLAYER_EVENT_TYPE.DZ_PLAYER_EVENT_QUEUELIST_LOADED && CurrentStation.IndexOf("playlist", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Next();
                    Console.WriteLine("Next song - plalist");
                }
                eventcb.Invoke(player, playerEvent);
            };

            ret = dz_player_set_event_cb(libcPlayerHndl, this.eventcb);
            Console.WriteLine($"dz_player_set_event_cb result: {ret}");

            return ret;
        }

        public ERRORS LoadStream(string url)
        {
            ERRORS ret;
            ret = dz_player_load(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero, url);

            if (ret == ERRORS.DZ_ERROR_NO_ERROR)
                CurrentStation = url;

            return ret;
        }

        public ERRORS Play()
        {
            ERRORS ret;
            ret = dz_player_play(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero, PLAYER_COMMANDS.START_TRACKLIST, DZ_INDEX_IN_QUEUELIST_CURRENT);

            if (ret == ERRORS.DZ_ERROR_NO_ERROR)
                IsPlaying = true;

            var lastState = NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_AWAYMODE_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
            return ret;
        }

        public ERRORS Stop()
        {
            ERRORS ret;
            ret = dz_player_stop(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero);

            if (ret == ERRORS.DZ_ERROR_NO_ERROR)
                IsPlaying = false;

            var lastState = NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            return ret;
        }

        public ERRORS Next()
        {
            ERRORS ret;
            ret = dz_player_play(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero, PLAYER_COMMANDS.START_TRACKLIST, DZ_INDEX_IN_QUEUELIST_NEXT);
            return ret;
        }

        public ERRORS Previous()
        {
            ERRORS ret;
            ret = dz_player_play(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero, PLAYER_COMMANDS.START_TRACKLIST, DZ_INDEX_IN_QUEUELIST_PREVIOUS);
            return ret;
        }

        public ERRORS Pause()
        {
            ERRORS ret;
            ret = dz_player_pause(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero);

            if (ret == ERRORS.DZ_ERROR_NO_ERROR)
                IsPlaying = false;

            var lastState = NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            return ret;
        }

        public ERRORS Dislike()
        {
            ERRORS ret;
            ret = dz_player_play(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero, PLAYER_COMMANDS.DISLIKE, DZ_INDEX_IN_QUEUELIST_NEXT);
            return ret;
        }

        public ERRORS Resume()
        {
            ERRORS ret;
            ret = dz_player_resume(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero);

            if (ret == ERRORS.DZ_ERROR_NO_ERROR)
                IsPlaying = true;

            NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_AWAYMODE_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
            return ret;
        }

        public ERRORS SetRepeatMode(QUEUELIST_REPEAT_MODE repeatMode)
        {
            ERRORS ret;
            ret = dz_player_set_repeat_mode(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero, repeatMode);
            return ret;
        }

        public ERRORS SetTrackQuality(DZ_TRACK_QUALITY trackQuality)
        {
            ERRORS ret;
            ret = dz_player_set_track_quality(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero, trackQuality);
            return ret;
        }

        public DZ_TRACK_QUALITY GetTrackQuality()
        {
            DZ_TRACK_QUALITY ret;
            ret = dz_player_get_track_quality(libcPlayerHndl);
            return ret;
        }

        public ERRORS Seek(ulong position)
        {
            ERRORS ret;
            ret = dz_player_seek(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero, position);
            return ret;
        }

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe PLAYER* dz_player_new(CONNECT* lpcc);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_set_event_cb(PLAYER* lpcc, libcPlayerOnEventCb cb);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_set_metadata_cb(PLAYER* lpcc, libcPlayerOnMetaDataCb cb);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_set_index_progress_cb(PLAYER* lpcc, libcPlayerOnIndexProgressCb cb, ulong refreshUs);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_set_render_progress_cb(PLAYER* lpcc, libcPlayerOnRenderProgressCb cb, ulong refreshUs);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_seek(PLAYER* lpcc, IntPtr cb, IntPtr userData, ulong positionMicroseconds);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_set_track_quality(PLAYER* lpcc, IntPtr cb, IntPtr userData, DZ_TRACK_QUALITY trackQuality);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_load_radio(PLAYER* lpcc, libcPlayerOnEventCb cb, IntPtr userData, string radioType, string radioId);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_cache_next(PLAYER* lpcc, libcPlayerOnEventCb cb, IntPtr userData, string trackUrl);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe DZ_TRACK_QUALITY dz_player_get_track_quality(PLAYER* lpcc);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_play_audioads(PLAYER* lpcc, libcPlayerOnEventCb cb, IntPtr operation_userdata);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_activate(PLAYER* dzPlayer, IntPtr userdata);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_load(PLAYER* dzPlayer, IntPtr cb, IntPtr userdata, string url);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_play(PLAYER* dzPlayer, IntPtr cb, IntPtr userdata, PLAYER_COMMANDS cmd, int mode);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_stop(PLAYER* dzPlayer, IntPtr cb, IntPtr userdata);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe bool dz_player_event_get_queuelist_context(PLAYER_EVENT* eventHendle, STREAMING_MODE* streamingMode, int* out_idx);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe bool dz_player_event_track_selected_is_preview(PLAYER_EVENT* eventHandle);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe bool dz_player_event_track_selected_rights(PLAYER_EVENT* eventHandle, bool* canPauseUnpause, bool* canSeek, int* numseek);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern unsafe IntPtr dz_player_event_track_selected_dzapiinfo(PLAYER_EVENT* eventHandle);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern unsafe IntPtr dz_player_event_track_selected_next_track_dzapiinfo(PLAYER_EVENT* eventHandle);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_pause(PLAYER* dzplayer, IntPtr cb, IntPtr userData);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_resume(PLAYER* dzplayer, IntPtr cb, IntPtr userData);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_set_repeat_mode(PLAYER* dzplayer, IntPtr cb, IntPtr userData, QUEUELIST_REPEAT_MODE mode);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_set_output_volume(PLAYER* dzplayer, IntPtr cb, IntPtr userData, uint volume);

        [DllImport("libdeezer.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ERRORS dz_player_deactivate(PLAYER* dzplayer, IntPtr cb, IntPtr userData);

        private void OnSongChanged(Song song)
        {
            SongChanged?.Invoke(this, song);
        }

        private void OnProgressChanged(ulong progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

        private void OnRenderChanged(ulong progress)
        {
            RenderChanged?.Invoke(this, progress);
        }

        public void Dispose()
        {
            dz_player_deactivate(libcPlayerHndl, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
