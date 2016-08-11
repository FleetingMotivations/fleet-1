using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using FleetEntityFramework.Models;
using FleetTransferObjects;

namespace FleetServer.Utils
{
    public static class Extensions
    {
        public static IReadOnlyList<T> GetValues<T>(this FleetHearbeatEnum me)
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static FleetHearbeatEnum RemoveFlag(this FleetHearbeatEnum beat, FleetHearbeatEnum flag)
        {
            beat &= ~flag;
            return beat;
        }

        public static FleetHearbeatEnum AddFlag(this FleetHearbeatEnum beat, FleetHearbeatEnum flag)
        {
            beat |= flag;
            return beat;
        }

    }

    public static class WorkstationExtentions
    {
        public static IQueryable<Workstation> GetNewWorkstations(this IQueryable<Workstation> workstations,
            IEnumerable<string> knownWorkstations)
        {
            // For now, until we define context, assume all attached workstations are part of the same 
            // "domain" or context
            // only get clients that have been seen in the last 1 hour
            var lastSeenMinimum = DateTime.Now.AddHours(-1);
            var candidateWorkstations = workstations
                .Where(w => w.LastSeen > lastSeenMinimum);

            var allWorkstationIds = workstations.Select(w => w.WorkstationIdentifier);
            var clientUnknowns = allWorkstationIds.Except(knownWorkstations);

            return workstations.Where(w => clientUnknowns.Contains(w.WorkstationIdentifier));
        }
    }
}