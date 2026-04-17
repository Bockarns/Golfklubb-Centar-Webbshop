# Golfklubb Centar – Webbshop

Ett skolprojekt utvecklat av ett team om 4 personer. En fullständig webbapplikation för Golfklubb Centar med webbshop, forum, adminpanel och communityfunktioner byggd med ASP.NET Core MVC och Entity Framework Core.

---

## Innehållsförteckning

- [Om projektet](#om-projektet)
- [Teamet](#teamet)
- [Tekniker och verktyg](#tekniker-och-verktyg)
- [Funktionalitet](#funktionalitet)
- [Projektstruktur](#projektstruktur)
- [Databasstruktur](#databasstruktur)
- [Kom igång](#kom-igång)
- [Konfiguration](#konfiguration)
- [Seeddata](#seeddata)
- [Roller och behörigheter](#roller-och-behörigheter)

---

## Om projektet

Golfklubb Centar Webbshop är en webbaserad plattform för en golfklubb där medlemmar kan handla golfprodukter, delta i ett community-forum, följa andra användare och få notifikationer om aktiviteter. Administratörer har tillgång till en komplett adminpanel för att hantera användare, produkter, kategorier, rabatter och beställningar.

---

## Teamet

| Namn   | Roll                    |
| ------ | ----------------------- |
| Anna   | Utvecklare              |
| Jerome | Scrum Master/Utvecklare |
| Ninos  | Utvecklare              |
| Peyman | Utvecklare              |

---

## Tekniker och verktyg

- **Framework:** ASP.NET Core MVC (.NET 9)
- **ORM:** Entity Framework Core 9
- **Databas:** Microsoft SQL Server
- **Autentisering:** ASP.NET Core Identity
- **Frontend:** Bootstrap 5, CSS, JavaScript
- **Versionshantering:** Git / GitHub
- **Projekthantering:** Jira
- **IDE:** Visual Studio 2022

### NuGet-paket

| Paket                                             | Version |
| ------------------------------------------------- | ------- |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 9.0.12  |
| Microsoft.AspNetCore.Identity.UI                  | 9.0.12  |
| Microsoft.EntityFrameworkCore.SqlServer           | 9.0.12  |
| Microsoft.EntityFrameworkCore.Tools               | 9.0.12  |
| Microsoft.VisualStudio.Web.CodeGeneration.Design  | 9.0.12  |

---

## Funktionalitet

### Webbshop

- Produktkatalog med bilder, beskrivningar och priser
- Filtrering per kategori (huvud- och underkategorier)
- Sortering av produkter
- Paginering
- Produktdetaljer med recensioner och betyg
- Varukorg (session-baserad)
- Checkout med leveransinformation
- Orderbekräftelse
- Rabattsystem kopplat till produkter

### Användarkonto (Identity)

- Registrering och inloggning
- Redigera profil (namn, användarnamn, email, telefon)
- Ladda upp profilbild
- Byta lösenord
- Orderhistorik med kvitton
- Publik profilsida

### Community – Forum

- Lista och bläddra bland trådar
- Skapa nya forumtrådar
- Kommentera på trådar
- Radera egna inlägg och kommentarer
- Forumblockering (blockerade användare kan inte posta/kommentera)

### Följ-system och notifikationer

- Följa och avfölja andra användare
- Lista över vem man följer och vem som följer en
- Bell-ikon i navbar med räknare för olästa notifikationer (uppdateras var 30:e sekund)
- Notifikationer för:
  - Ny följare
  - Ny forumtråd från en följd användare
  - Ny kommentar på en tråd man skapat eller deltagit i
  - Admin har raderat ens inlägg eller kommentar
  - Orderstatus har ändrats

### Adminpanel

Tillgänglig via `/Admin` för användare med rollen Admin.

**Dashboard**

- Counters för orderstatus (Ny Order, Packas, Skickad, Levererad, Avbruten)
- Aktivitetsöversikt för senaste 30 dagarna (nya trådar, kommentarer, recensioner)
- Senaste ordrar och forumtrådar

**Användarhantering**

- Lista alla användare
- Visa användardetaljer
- Redigera användaruppgifter (namn, email, användarnamn, telefon, roller)
- Radera och ladda upp profilbild
- Blockera/avblockera användare från forumet
- Radera användare
- Visa användarens orderhistorik
- Visa användarens forumhistorik

**Webbshop-administration**

- CRUD för produkter (inkl. bilduppladdning och lagerhantering)
- CRUD för kategorier (med stöd för föräldrakategorier)
- CRUD för rabatter

**Orderhantering**

- Lista alla inkommande beställningar
- Visa orderdetaljer
- Uppdatera orderstatus (Ny Order → Packas → Skickad → Levererad / Avbruten)

---

## Projektstruktur

```
Golfklubb-Centar-Webbshop/
├── Areas/
│   └── Identity/
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── ApplicationUser.cs
│       │   └── SeedData.cs
│       └── Pages/
│           └── Account/
│               └── Manage/         ← Användarprofil, orderhistorik, kvitto
├── Controllers/
│   ├── AdminCategoryController.cs
│   ├── AdminController.cs          ← Dashboard + Webshop-landingpage
│   ├── AdminDiscountController.cs
│   ├── AdminOrderController.cs
│   ├── AdminProductController.cs
│   ├── AdminUserController.cs
│   ├── CartController.cs
│   ├── FollowController.cs
│   ├── ForumController.cs
│   ├── HomeController.cs
│   ├── NotificationController.cs
│   ├── UserController.cs           ← Publika användarprofiler
│   └── WebshopController.cs
├── Models/
│   ├── CartItem.cs                 ← Session-modell (ingen DB-tabell)
│   ├── Category.cs
│   ├── Comment.cs
│   ├── Discount.cs
│   ├── Follow.cs
│   ├── History.cs
│   ├── Invoice.cs
│   ├── InvoiceItem.cs
│   ├── Notification.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Payment.cs
│   ├── Post.cs
│   ├── Product.cs
│   ├── ProductReview.cs
│   ├── ReviewReply.cs
│   ├── Stock.cs
│   ├── Taxis.cs
│   └── ViewModels/                 ← Alla ViewModels
├── Views/
│   ├── Admin/
│   ├── AdminCategory/
│   ├── AdminDiscount/
│   ├── AdminOrder/
│   ├── AdminProduct/
│   ├── AdminUser/
│   ├── Cart/
│   ├── Follow/
│   ├── Forum/
│   ├── Home/
│   ├── Notification/
│   ├── Shared/
│   ├── User/
│   └── Webshop/
├── wwwroot/
│   ├── css/
│   ├── images/
│   └── js/
├── Migrations/
├── Program.cs
└── appsettings.json
```

---

## Databasstruktur

Databasen är uppdelad i fyra scheman:

| Schema              | Tabeller                                                                                       |
| ------------------- | ---------------------------------------------------------------------------------------------- |
| `CentarUserMngt`    | Users, Roles, UserRoles, UserClaims, UserLogin, UserTokens, RoleClaims, Follows, Notifications |
| `CentarProductMngt` | Products, Categories, Discounts, Stocks, ProductReviews                                        |
| `CentarOrderMngt`   | Orders, OrderItems, Invoices, InvoiceItems, Histories, Payments, Taxes                         |
| `CentarForumMngt`   | Posts, Comments                                                                                |

### Varukorg

Varukorgen är session-baserad och sparas inte i databasen under shoppingprocessen. Vid checkout skapas `Order` → `OrderItems` direkt i databasen.

Databasen är förberedd med Invoices, InvoiceItems, Payment, Taxes och History.

---

## Kom igång

### Krav

- Visual Studio 2022 eller senare
- .NET 9 SDK
- SQL Server (eller åtkomst till projektets live-databas)

### Installation

1. Klona repot:

```bash
git clone https://github.com/[organisation]/Golfklubb-Centar.git
```

2. Öppna `Golfklubb-Centar-Webbshop.slnx` i Visual Studio.

3. Uppdatera connection string i `appsettings.json` om du kör en lokal databas (se [Konfiguration](#konfiguration)).

4. Om du ansluter till projektets live-databas behöver du inte köra migrationer — databasen är redan uppdaterad.

5. Kör projektet med `F5` eller `dotnet run`.

### Lokalt databas-setup (valfritt)

Om du vill köra en lokal databas istället för live-databasen:

```
Update-Database
```

Seeddata för roller, rabatter, kategorier och produkter kan aktiveras i `Program.cs` genom att avkommentera seedning-blocket.

---

## Konfiguration

Connection string konfigureras i `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ApplicationDbContextConnection": "Din connection string här"
  }
}
```

> **OBS:** Lägg aldrig in känslig information som lösenord direkt i `appsettings.json` i ett produktionsprojekt. Använd miljövariabler eller Azure Key Vault.

### Sessions

Sessions är konfigurerade med 30 minuters timeout och används för varukorgen.

---

## Seeddata

`SeedData.cs` innehåller metoder för att sätta upp grunddata:

| Metod          | Beskrivning                                                     |
| -------------- | --------------------------------------------------------------- |
| `SeedRoles`    | Skapar rollerna `Admin` och `User`, samt ett standardadminkonto |
| `SeedDiscount` | Skapar grundläggande rabatter (Ordinarie Pris, StartRea)        |
| `SeedCategory` | Skapar testkategorier                                           |
| `SeedProduct`  | Skapar exempelprodukter                                         |

### Standardadminkonto (efter seeding)

```
Email:    admin@test.se
Lösenord: Test123!
```

---

## Roller och behörigheter

| Roll            | Behörigheter                                                                                              |
| --------------- | --------------------------------------------------------------------------------------------------------- |
| **Admin**       | Full tillgång till adminpanelen, kan hantera användare, produkter, kategorier, rabatter och beställningar |
| **User**        | Kan handla, delta i forum, följa användare, skriva recensioner och hantera sin profil                     |
| **Ej inloggad** | Kan bläddra i webbshopen och läsa forumtrådar                                                             |

---

## Migreringshistorik

| Migration                                   | Beskrivning                                     |
| ------------------------------------------- | ----------------------------------------------- |
| Initial                                     | Grundläggande tabellstruktur                    |
| UpdateSchemaNameAndTableNames               | Uppdaterade schemanamn                          |
| AdditionTablesAndRelationsToApplicationUser | Lade till relationer till ApplicationUser       |
| AddForumBanAndProfileImagePathToUser        | IsForumBanned och ProfileImagePath på användare |
| CreatedAtForReview                          | Datum på recensioner                            |
| DroppedCartTablesAndAddedNewPropToOrder     | Session-baserad varukorg, nya orderfält         |
| OrderItemModelAndFullNameToOrder            | OrderItem-modell och FullName på Order          |
| DatePropForOrder                            | StatusDate på Order                             |
| AddedReviewReplyModel                       | Svar på recensioner                             |
| AddFollowAndNotifications                   | Följ-system och notifikationer                  |
| AddLinkToNotification                       | Klickbara notifikationslänkar                   |
| UpdateForumColumnsToUnicode                 | Unicode-stöd för emojis i forum                 |
| CityAndCountryAddedToApplicationUser        | Stad och land på användarprofil                 |
