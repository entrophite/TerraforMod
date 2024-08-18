# TerraforMod

A former BepInEx-based gameplay mod for *Terraformers* now using static code injection. Thanks to the anti-mod measures implemented by the devs, it's not ideal to keep modding this game using BepInEx any more. The so-called "anti-mod" measure is probably as rudimentary as just stripping unused symbols to make decompiling, recompiling and code analysis a bit harder. The current game is shipped with stripped system runtime libraries (e.g. System.dll) as well as stripped game code assemblies. After consideration I chose to statically inject code into the game's assemblies. Fortunately, the game's slow update cycle makes such static code injection sweet. Finally, anti-modding and anti-cheating in single-player games is anyway meaningless and stupid.

<b><span style="color:#60ff60">VERSION: </span></b> 1.4.11@steam

<b><span style="color:#d0d010">NOTE: </span></b> The game's main code assembly is _GameAssembly.dll.

## 1. Production

### 1.1. Production multiplier

<b><span style="color:#4040ff">SYNOPSIS: </span></b> Modifying the production per cycle from region and city buildings, 10x in the example below. It doesn't work with lifeforms.

```c#
// Terraf.ProjectsService.GetProjectProds
// multiple lines in this function, in Line 45, 58, 73, 147, 175, 637, 676, 762, 770, 779, 788, 796, 881, 918, 930, 942, 954, 966
// example as below around Line 45
...
if ((!flag4 || flag6) && !flag7)
{
	ResourceProd item = default(ResourceProd);
	item.Resource = projectById.ResourceProductions[i].type;
	item.Source = ProdSource.Default;
	item.Amount = projectById.ResourceProductions[i].amount * 10;  // Line 45
	CS$<>8__locals1.prods.Add(item);
}
...

// Terraf.ProjectsService.<GetProjectProds>g__AddOwnAbilityProd|40_0
internal static void <GetProjectProds>g__AddOwnAbilityProd|40_0(ResourceType type, int amount, ref ProjectsService.<>c__DisplayClass40_0 A_2)
{
	if (amount == 0)
	{
		return;
	}
	ResourceProd item = default(ResourceProd);
	item.Resource = type;
	item.Amount = amount * 10;
	item.Source = ProdSource.OwnAbility;
	A_2.prods.Add(item);
}
```