using System.Collections.Generic;
using DSharpPlus.Lavalink;

namespace AudioLibrary
{
    public static class PlaysoundQueue
    {
        private static Dictionary<ulong, Queue<LavalinkTrack>> _guildQueue =
            new Dictionary<ulong, Queue<LavalinkTrack>>();

        public static int QueuedTracks(ulong guildId) => _guildQueue.ContainsKey(guildId) ? _guildQueue[guildId].Count : 0;

        public static bool TryGetTracks(ulong guildId, out Queue<LavalinkTrack> tracks)
        {
            return _guildQueue.TryGetValue(guildId, out tracks);
        }

        public static void EnqueueTrack(ulong guildId, LavalinkTrack track)
        {
            if (!_guildQueue.ContainsKey(guildId)) _guildQueue.Add(guildId, new Queue<LavalinkTrack>());
            
            _guildQueue[guildId].Enqueue(track);
        }

        public static LavalinkTrack DequeueTrack(ulong guildId)
        {
            if (!_guildQueue.ContainsKey(guildId)) return null;

            var track = _guildQueue[guildId].Dequeue();
            if (_guildQueue[guildId].Count == 0)
            {
                _guildQueue.Remove(guildId);
            }

            return track;
        }
    }
}