using Weapons.Abstract;

namespace Weapons.Definitions {
    public class Pistol : WeaponBase {
        protected override void ShootAction() => ShootForwardWithDeviation(10f);
    }
}