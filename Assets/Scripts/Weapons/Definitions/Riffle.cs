using Weapons.Abstract;

namespace Weapons.Definitions {
    public class Riffle : WeaponBase {
        public override void ShootAction() => ShootBurst(25, 10);
    }
}