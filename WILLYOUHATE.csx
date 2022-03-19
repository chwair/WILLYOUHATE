using System.Linq;

EnsureDataLoaded();


static Random rng;
rng = new Random(int.Parse(SimpleTextInput("Enter Seed", "Enter Seed: Make sure it's a number from -2147483648 to 2147483647", "0", false)));
float randomPower = float.Parse(SimpleTextInput("Shuffle Power", "Shuffle Power: Make sure it's a number from 0 to 1", "0.6", false));
int spriteCorrLevel = int.Parse(SimpleTextInput("Sprite Corruption Level", "Corruption level: 1 = Shelly and walls are ignored. 2 = Walls are ignored. 3 = Nothing is ignored, seek help.", "2", false));
if (spriteCorrLevel == 3)
{
    ScriptMessage("You selected Corruption Level 3. This corruption level is near impossible to complete without exploration mode. May Squid Bless You.");
}
static void Shuffle<T>(this IList<T> list)
{
    int n = list.Count;
    while (n > 1)
    {
        n--;
        int k = rng.Next(n + 1);

        T value = list[k];
        list[k] = list[n];
        list[n] = value;
    }
}

static void ShuffleOnlySelected<T>(this IList<T> list, IList<int> selected, Action<int, int> swapFunc)
{
    int n = selected.Count;
    while (n > 1)
    {
        n--;
        int k = rng.Next(n + 1);

        swapFunc(selected[n], selected[k]);

        int idx = selected[k];
        selected[k] = selected[n];
        selected[n] = idx;
    }
}

static void ShuffleOnlySelected<T>(this IList<T> list, IList<int> selected)
{
    list.ShuffleOnlySelected(selected, (n, k) => 
    {
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
    });
}

static void SelectSome(this IList<int> list, float amountToKeep)
{
    int toRemove = (int)(list.Count * (1 - amountToKeep));
    for (int i = 0; i < toRemove; i++)
        list.RemoveAt(rng.Next(list.Count));
}

List<int> tiny = new List<int>();
List<int> small = new List<int>();
List<int> characterlike = new List<int>();
List<int> big = new List<int>();
for (int i = 0; i < Data.Sprites.Count; i++)
{
    var sprite = Data.Sprites[i];
    if (spriteCorrLevel == 1)
    {
        if (sprite.Name.Content.StartsWith("font_"))
            continue;
        if (sprite.Name.Content.StartsWith("spr_wall"))
            continue; // These 2 lines stop the thingy from not fucking up the walls
        if (sprite.Name.Content.StartsWith("spr_player"))
            continue; // These 2 lines stop the thingy from not fucking up the player
    }
    else if (spriteCorrLevel == 2)
    {
        if (sprite.Name.Content.StartsWith("font_"))
            continue;
        if (sprite.Name.Content.StartsWith("spr_wall"))
            continue; // These 2 lines stop the thingy from not fucking up the walls (again)
    }
    if (sprite.Width < 50 && sprite.Height < 50)
        tiny.Add(i);
    else if (sprite.Width < 50 && sprite.Height < 100)
        characterlike.Add(i);
    else if (sprite.Width < 100 && sprite.Height < 100)
        small.Add(i);
    else if (sprite.Width < 200 && sprite.Height < 200)
        big.Add(i);
}
tiny.SelectSome(randomPower);
small.SelectSome(randomPower);
characterlike.SelectSome(randomPower);
big.SelectSome(randomPower);
Data.Sprites.ShuffleOnlySelected(tiny);
Data.Sprites.ShuffleOnlySelected(small);
Data.Sprites.ShuffleOnlySelected(characterlike);
Data.Sprites.ShuffleOnlySelected(big);

Data.Sounds.Shuffle();

Data.Strings.Shuffle();

foreach (var obj in Data.GameObjects)
{
    if (!obj.Visible)
        continue;
    if (obj._Sprite.CachedId >= 0)
        obj.Sprite = Data.Sprites[obj._Sprite.CachedId];
    if (obj._TextureMaskId.CachedId >= 0)
        obj.TextureMaskId = Data.Sprites[obj._TextureMaskId.CachedId];
}

ScriptMessage("Simulation corrupted. Have fun!");
