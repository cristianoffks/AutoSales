using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoSales.Infrastructure.Migrations
{
    public partial class SimplifyVehicleBrandAndModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Vehicles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Vehicles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                EXEC(N'
                    UPDATE vehicle
                    SET vehicle.Brand = brand.Name, vehicle.Model = model.Name
                    FROM Vehicles vehicle
                    INNER JOIN VehicleModels model ON vehicle.ModelId = model.Id
                    INNER JOIN Brands brand ON model.BrandId = brand.Id;
                ');
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleModels");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ModelId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Vehicles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModelId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleModels_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ModelId",
                table: "Vehicles",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_BrandId_Name",
                table: "VehicleModels",
                columns: new[] { "BrandId", "Name" },
                unique: true);

            migrationBuilder.Sql("""
                EXEC(N'
                    INSERT INTO Brands (Name)
                    SELECT DISTINCT Brand FROM Vehicles;

                    INSERT INTO VehicleModels (BrandId, Name)
                    SELECT DISTINCT brand.Id, vehicle.Model
                    FROM Vehicles vehicle
                    INNER JOIN Brands brand ON vehicle.Brand = brand.Name;

                    UPDATE vehicle
                    SET vehicle.ModelId = model.Id
                    FROM Vehicles vehicle
                    INNER JOIN Brands brand ON vehicle.Brand = brand.Name
                    INNER JOIN VehicleModels model ON model.BrandId = brand.Id AND model.Name = vehicle.Model;
                ');
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles",
                column: "ModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Vehicles");
        }
    }
}
