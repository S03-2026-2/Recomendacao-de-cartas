using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Models;

namespace PokemonRecommender.Data;

public static class AdminSeeder
{
    // Admin de demonstração, criado só em desenvolvimento. O token dele é o próprio id.
    public static readonly Guid AdminDemoId = Guid.Parse("3f2b8c1e-0000-4000-8000-0000000000ad");

    public static async Task GarantirAdminDemoAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.Usuarios.OfType<Admin>().AnyAsync(a => a.Id == AdminDemoId, ct))
            return;

        db.Usuarios.Add(new Admin(AdminDemoId, "Administrador (demo)"));
        await db.SaveChangesAsync(ct);
    }
}