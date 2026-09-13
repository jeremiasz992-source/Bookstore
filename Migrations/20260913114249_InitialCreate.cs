using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Publisher = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                    table.ForeignKey(
                        name: "FK_Books_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartId = table.Column<int>(type: "integer", nullable: false),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItems_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProviderTransactionId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name" },
                values: new object[] { 1, "Bez kategorii" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordHash", "Role" },
                values: new object[] { 1, "test-user@xyz.com", "Test User", "AQAAAAIAAYagAAAAEN9U7byHyGamN3kRb/OSKRVNeKKhpr9fzpROUZZaotSltUD8Vm7y3JDZ3CwJP10Z/w==", "Admin" });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "CartId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name" },
                values: new object[,]
                {
                    { 2, "Programowanie" },
                    { 3, "Historia" },
                    { 4, "Science fiction" },
                    { 5, "Biznes" },
                    { 6, "Literatura piękna" },
                    { 7, "Fantasy" },
                    { 8, "Kryminał" },
                    { 9, "Dla dzieci" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Title", "Author", "Publisher", "Year", "Price", "Stock", "CategoryId", "CoverImageUrl", "Description" },
                values: new object[,]
                {
                    { 1, "ASP.NET Core Web API", "Tomasz Baran", "Tech Press", 2024, 89.00m, 9, 2, "/covers/aspnet-core-web-api.svg", "Wprowadzenie do projektowania i budowania aplikacji Web API w ASP.NET Core." },
                    { 2, "Czysty Kod w C#", "Piotr Wiśniewski", "Tech Press", 2023, 79.00m, 8, 2, "/covers/czysty-kod-w-csharp.svg", "Praktyczny przewodnik po pisaniu czytelnego i utrzymywalnego kodu w C#." },
                    { 3, "Krótka Historia Handlu", "Paweł Wolski", "Historia Prosta", 2017, 42.00m, 11, 3, "/covers/krotka-historia-handlu.svg", "Przystępny przegląd rozwoju handlu od dawnych targów po e-commerce." },
                    { 4, "Skuteczny Sklep Internetowy", "Monika Kaczmarek", "Biznes Plus", 2021, 54.90m, 14, 5, "/covers/skuteczny-sklep-internetowy.svg", "Podstawy prowadzenia sklepu online, sprzedaży i pracy z klientem." },
                    { 5, "Miasto o Poranku", "Katarzyna Lis", "Literackie Studio", 2018, 29.99m, 10, 6, "/covers/miasto-o-poranku.svg", "Literacka historia o codziennych wyborach, relacjach i nowym początku." },
                    { 6, "Cień Starego Lasu", "Anna Nowak", "Nova Books", 2021, 39.99m, 15, 7, "/covers/cien-starego-lasu.svg", "Przygodowa opowieść fantasy o tajemniczym lesie i dawnej magii." },
                    { 7, "Smok z Północnej Bramy", "Jan Kowalski", "Nova Books", 2020, 44.90m, 12, 7, "/covers/smok-z-polnocnej-bramy.svg", "Historia wyprawy do północnej twierdzy, gdzie legenda o smoku okazuje się prawdą." },
                    { 8, "Ostatni Trop", "Ewa Maj", "Crime House", 2022, 37.90m, 18, 8, "/covers/ostatni-trop.svg", "Dynamiczna powieść kryminalna o sprawie, która wraca po latach." },
                    { 9, "Zaginiony Manuskrypt", "Marek Zieliński", "Crime House", 2019, 34.50m, 20, 8, "/covers/zaginiony-manuskrypt.svg", "Kryminał o skradzionym rękopisie i śledztwie prowadzonym wśród kolekcjonerów." },
                    { 10, "Przygody Małego Odkrywcy", "Julia Król", "Dziecięce Strony", 2022, 24.99m, 25, 9, "/covers/przygody-malego-odkrywcy.svg", "Lekka książka dla dzieci o ciekawości świata i pierwszych wyprawach." },
                    { 11, "Szept Starego Miasta", "Łukasz Dąbrowski", "Crime House", 2021, 41.00m, 6, 8, "/covers/szept-starego-miasta.svg", "Kryminał osadzony w zabytkowej dzielnicy, gdzie każdy mieszkaniec ma coś do ukrycia." },
                    { 12, "Kod i Kawa", "Robert Mazur", "Tech Press", 2023, 59.00m, 12, 2, "/covers/kod-i-kawa.svg", "Zbiór praktycznych wskazówek dla programistów rozpoczynających pracę w zespole." },
                    { 13, "Sekrety Negocjacji", "Natalia Wieczorek", "Biznes Plus", 2022, 49.90m, 9, 5, "/covers/sekrety-negocjacji.svg", "Przewodnik po prowadzeniu rozmów handlowych i budowaniu trwałych relacji z klientem." },
                    { 14, "Wiatr nad Zatoką", "Agnieszka Sikora", "Literackie Studio", 2020, 34.50m, 7, 6, "/covers/wiatr-nad-zatoka.svg", "Powieść obyczajowa o powrocie do nadmorskiego miasteczka i rozliczeniu z przeszłością." },
                    { 15, "Mapa Nieznanych Wysp", "Grzegorz Pawlak", "Dziecięce Strony", 2023, 27.90m, 15, 9, "/covers/mapa-nieznanych-wysp.svg", "Książka dla dzieci o wyprawie w poszukiwaniu wysp, których nie ma na żadnej mapie." },
                    { 16, "Srebrne Ostrze", "Adam Wróbel", "Nova Books", 2019, 39.99m, 6, 7, "/covers/srebrne-ostrze.svg", "Opowieść fantasy o wędrownym wojowniku, który przyjmuje zlecenia odrzucane przez innych." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_BookId",
                table: "CartItems",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_BookId",
                table: "CartItems",
                columns: new[] { "CartId", "BookId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_BookId",
                table: "OrderItems",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
