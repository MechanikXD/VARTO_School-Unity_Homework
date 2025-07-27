using Weapons.Abstract;

namespace Weapons.Definitions {
    public class Riffle : WeaponBase {
        protected override void ShootAction() => ShootForwardWithDeviation(10f);
    }
}