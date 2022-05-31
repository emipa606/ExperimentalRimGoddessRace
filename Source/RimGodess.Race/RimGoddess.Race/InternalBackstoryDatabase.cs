using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RimGoddess.Race;

[StaticConstructorOnStartup]
public static class InternalBackstoryDatabase
{
    private static readonly Dictionary<string, Backstory> GoddessBackstories;

    private static readonly Dictionary<string, Backstory> HandmaidenBackstories;

    static InternalBackstoryDatabase()
    {
        GoddessBackstories = new Dictionary<string, Backstory>();
        HandmaidenBackstories = new Dictionary<string, Backstory>();
        foreach (var goddessBackstory in GetGoddessBackstories())
        {
            DeepProfiler.Start("RimGoddess.GoddessBackstory.PostLoad");
            try
            {
                goddessBackstory.PostLoad();
            }
            finally
            {
                DeepProfiler.End();
            }

            DeepProfiler.Start("RimGoddess.GoddessBackstory.ResolveReferences");
            try
            {
                goddessBackstory.ResolveReferences();
            }
            finally
            {
                DeepProfiler.End();
            }

            foreach (var item in goddessBackstory.ConfigErrors(false))
            {
                Log.Error($"RimGoddess - Race: {goddessBackstory.title}: {item}");
            }

            if (!GoddessBackstories.ContainsKey(goddessBackstory.identifier))
            {
                Log.Message($"RimGoddess - Race: Added Goddess Backstory {goddessBackstory.identifier}");
                GoddessBackstories.Add(goddessBackstory.identifier, goddessBackstory);
            }
            else
            {
                Log.Warning($"RimGoddess - Race: Duplicate Goddess Backstory {goddessBackstory.identifier}");
            }
        }

        foreach (var maidenBackstory in GetMaidenBackstories())
        {
            DeepProfiler.Start("RimGoddess.HandmaidenBackstory.PostLoad");
            try
            {
                maidenBackstory.PostLoad();
            }
            finally
            {
                DeepProfiler.End();
            }

            DeepProfiler.Start("RimGoddess.HandmaidenBackstory.ResolveReferences");
            try
            {
                maidenBackstory.ResolveReferences();
            }
            finally
            {
                DeepProfiler.End();
            }

            foreach (var item2 in maidenBackstory.ConfigErrors(false))
            {
                Log.Error($"RimGoddess - Race: {maidenBackstory.title}: {item2}");
            }

            if (!GoddessBackstories.ContainsKey(maidenBackstory.identifier))
            {
                Log.Message($"RimGoddess - Race: Added Handmaiden Backstory: {maidenBackstory.identifier}");
                HandmaidenBackstories.Add(maidenBackstory.identifier, maidenBackstory);
            }
            else
            {
                Log.Warning($"RimGoddess - Race: Duplicate Handmaiden Backstory: {maidenBackstory.identifier}");
            }
        }

        XmlInheritance.Clear();
    }

    private static IEnumerable<Backstory> GetBackstories(string a_targetDestination)
    {
        var content = RimGoddessRace.Instance.Content;
        XmlInheritance.Clear();
        var list = new List<LoadableXmlAsset>();
        var array = DirectXmlLoader.XmlAssetsInModFolder(content, a_targetDestination);
        foreach (var loadableXmlAsset in array)
        {
            XmlInheritance.TryRegisterAllFrom(loadableXmlAsset, content);
            list.Add(loadableXmlAsset);
        }

        XmlInheritance.Resolve();
        foreach (var item in list)
        {
            foreach (var item2 in DirectXmlLoader.AllGameItemsFromAsset<Backstory>(item))
            {
                yield return item2;
            }
        }
    }

    private static IEnumerable<Backstory> GetGoddessBackstories()
    {
        return GetBackstories("Backstories/Goddess");
    }

    private static IEnumerable<Backstory> GetMaidenBackstories()
    {
        return GetBackstories("Backstories/Handmaiden");
    }

    public static Backstory GetMaidenBackstory(string a_name)
    {
        return HandmaidenBackstories[a_name];
    }

    public static Backstory GetGoddessBackstory(string a_name)
    {
        return GoddessBackstories[a_name];
    }

    public static Backstory RandomGoddessBackstory(BackstorySlot a_slot)
    {
        return GoddessBackstories.Where(bs => bs.Value.slot == a_slot).RandomElement().Value;
    }

    public static Backstory RandomMaidenBackstory(BackstorySlot a_slot)
    {
        return HandmaidenBackstories.Where(bs => bs.Value.slot == a_slot).RandomElement().Value;
    }
}