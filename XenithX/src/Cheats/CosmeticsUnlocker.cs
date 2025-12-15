namespace ZenithX;

    public static class CosmeticsUnlocker
    {
        public static void unlockCosmetics(HatManager hatManager)
        {
            if (hatManager == null || !CheatToggles.freeCosmetics) return;

            foreach (var bundle in hatManager.allBundles) bundle.Free = true;
            foreach (var featuredBundle in hatManager.allFeaturedBundles) featuredBundle.Free = true;
            foreach (var featuredCube in hatManager.allFeaturedCubes) featuredCube.Free = true;
            foreach (var featuredItem in hatManager.allFeaturedItems) featuredItem.Free = true;
            foreach (var hat in hatManager.allHats) hat.Free = true;
            foreach (var nameplate in hatManager.allNamePlates) nameplate.Free = true;
            foreach (var pet in hatManager.allPets) pet.Free = true;
            foreach (var skin in hatManager.allSkins) skin.Free = true;
            foreach (var starBundle in hatManager.allStarBundles) starBundle.price = 0;
            foreach (var visor in hatManager.allVisors) visor.Free = true;
        }
    }