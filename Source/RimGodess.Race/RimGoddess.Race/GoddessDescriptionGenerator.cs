using System.Collections.Generic;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace RimGoddess.Race;

[StaticConstructorOnStartup]
public static class GoddessDescriptionGenerator
{
    private static readonly List<string> Names;

    private static readonly Dictionary<string, Details> GoddessNames;

    static GoddessDescriptionGenerator()
    {
        GoddessNames = new Dictionary<string, Details>
        {
            {
                "Rouge",
                new Details
                {
                    TName = "Red",
                    HighlightColor = new Color(1f, 0f, 0f)
                }
            },
            {
                "Vert",
                new Details
                {
                    TName = "Green",
                    HighlightColor = new Color(0f, 1f, 0f)
                }
            },
            {
                "Bleue",
                new Details
                {
                    TName = "Blue",
                    HighlightColor = new Color(0f, 0f, 1f)
                }
            },
            {
                "Marron",
                new Details
                {
                    TName = "Brown",
                    HighlightColor = new Color(0.65f, 0.16f, 0.16f)
                }
            },
            {
                "Cramoisie",
                new Details
                {
                    TName = "Crimson",
                    HighlightColor = new Color(0.86f, 0.08f, 0.24f)
                }
            },
            {
                "Grise",
                new Details
                {
                    TName = "Gray",
                    HighlightColor = new Color(0.5f, 0.5f, 0.5f)
                }
            },
            {
                "Orchidee",
                new Details
                {
                    TName = "Orchid",
                    HighlightColor = new Color(0.85f, 0.44f, 0.84f)
                }
            },
            {
                "Rose",
                new Details
                {
                    TName = "Pink",
                    HighlightColor = new Color(1f, 0.75f, 0.8f)
                }
            },
            {
                "Or",
                new Details
                {
                    TName = "Gold",
                    HighlightColor = new Color(1f, 0.84f, 0f)
                }
            },
            {
                "Ivoire",
                new Details
                {
                    TName = "Ivory",
                    HighlightColor = new Color(1f, 1f, 0.94f)
                }
            },
            {
                "Kaki",
                new Details
                {
                    TName = "Khaki",
                    HighlightColor = new Color(0.94f, 0.9f, 0.55f)
                }
            },
            {
                "Lavande",
                new Details
                {
                    TName = "Lavender",
                    HighlightColor = new Color(0.9f, 0.9f, 0.98f)
                }
            },
            {
                "Bordeaux",
                new Details
                {
                    TName = "Maroon",
                    HighlightColor = new Color(0.5f, 0f, 0f)
                }
            },
            {
                "Marine",
                new Details
                {
                    TName = "Navy",
                    HighlightColor = new Color(0f, 0f, 0.5f)
                }
            },
            {
                "Violette",
                new Details
                {
                    TName = "Violet",
                    HighlightColor = new Color(0.93f, 0.51f, 0.93f)
                }
            },
            {
                "Perou",
                new Details
                {
                    TName = "Peru",
                    HighlightColor = new Color(0.8f, 0.52f, 0.25f)
                }
            },
            {
                "Argent",
                new Details
                {
                    TName = "Silver",
                    HighlightColor = new Color(0.75f, 0.75f, 0.75f)
                }
            },
            {
                "Neige",
                new Details
                {
                    TName = "Snow",
                    HighlightColor = new Color(1f, 0.98f, 0.98f)
                }
            },
            {
                "Sarcelle",
                new Details
                {
                    TName = "Teal",
                    HighlightColor = new Color(0f, 0.5f, 0.5f)
                }
            },
            {
                "Blanc",
                new Details
                {
                    TName = "White",
                    HighlightColor = new Color(1f, 1f, 1f)
                }
            },
            {
                "Noire",
                new Details
                {
                    TName = "Black",
                    HighlightColor = new Color(0f, 0f, 0f)
                }
            }
        };
        Names = new List<string>();
        foreach (var key in GoddessNames.Keys)
        {
            Names.Add(key);
        }
    }

    public static string GenerateName()
    {
        var random = new Random();
        for (var i = 0; i < 100; i++)
        {
            var text = Names[random.Next(Names.Count)];
            if (!new NameSingle(text).UsedThisGame)
            {
                return text;
            }
        }

        return "Red";
    }

    public static string GetTransformedName(string a_name)
    {
        if (GoddessNames.ContainsKey(a_name))
        {
            return GoddessNames[a_name].TName;
        }

        return null;
    }

    public static Color GetColor(string a_name)
    {
        return GoddessNames[a_name].HighlightColor;
    }

    private struct Details
    {
        public string TName;

        public Color HighlightColor;
    }
}