using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    public partial class hotel1229 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Administrations",
                columns: table => new
                {
                    ID = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Account = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrations", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CompanyGroups",
                columns: table => new
                {
                    ID = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GroupName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyGroups", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmployeeInstances",
                columns: table => new
                {
                    ID = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmployeeName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdentityCardType = table.Column<int>(type: "int", nullable: false),
                    IdentityCardId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Character = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeInstances", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NickName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Account = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IDNumber = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Coupon = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Info = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subscription = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Points = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HotelInstances",
                columns: table => new
                {
                    ID = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompanyGroupID = table.Column<uint>(type: "int unsigned", nullable: false),
                    HotelName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HotelAddress = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactList = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelInstances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HotelInstances_CompanyGroups_CompanyGroupID",
                        column: x => x.CompanyGroupID,
                        principalTable: "CompanyGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    ID = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<uint>(type: "int unsigned", nullable: false),
                    Account = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Consignee = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Goods = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GoodsPoints = table.Column<uint>(type: "int unsigned", nullable: false),
                    DeliveryAddress = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactNumber = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Awards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Awards_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmployeeInstanceHotelInstance",
                columns: table => new
                {
                    EmployeeInstancesID = table.Column<uint>(type: "int unsigned", nullable: false),
                    HotelInstancesID = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeInstanceHotelInstance", x => new { x.EmployeeInstancesID, x.HotelInstancesID });
                    table.ForeignKey(
                        name: "FK_EmployeeInstanceHotelInstance_EmployeeInstances_EmployeeInst~",
                        column: x => x.EmployeeInstancesID,
                        principalTable: "EmployeeInstances",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeInstanceHotelInstance_HotelInstances_HotelInstancesID",
                        column: x => x.HotelInstancesID,
                        principalTable: "HotelInstances",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    ID = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoomStatus = table.Column<int>(type: "int", nullable: false),
                    RoomType = table.Column<int>(type: "int", nullable: false),
                    HotelInstanceID = table.Column<uint>(type: "int unsigned", nullable: false),
                    Price = table.Column<uint>(type: "int unsigned", nullable: false),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ichnography = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Area = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GuestRoomType = table.Column<int>(type: "int", nullable: true),
                    BedCount = table.Column<uint>(type: "int unsigned", nullable: true),
                    WindowCount = table.Column<uint>(type: "int unsigned", nullable: true),
                    MineralWaterCount = table.Column<uint>(type: "int unsigned", nullable: true),
                    CondomCount = table.Column<uint>(type: "int unsigned", nullable: true),
                    EquipmentType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EquipmentCount = table.Column<uint>(type: "int unsigned", nullable: true),
                    WasherCount = table.Column<uint>(type: "int unsigned", nullable: true),
                    SeatCount = table.Column<uint>(type: "int unsigned", nullable: true),
                    SocketCount = table.Column<uint>(type: "int unsigned", nullable: true),
                    StaffRoom_BedCount = table.Column<uint>(type: "int unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Room_HotelInstances_HotelInstanceID",
                        column: x => x.HotelInstanceID,
                        principalTable: "HotelInstances",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    ID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UUID = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    PlatOrderNumber = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Payload = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<uint>(type: "int unsigned", nullable: false),
                    RefoundValue = table.Column<uint>(type: "int unsigned", nullable: false),
                    ProduceTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ReserveCheckInTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ReserveCheckOutTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    GuestRoomID = table.Column<uint>(type: "int unsigned", nullable: false),
                    UserID = table.Column<uint>(type: "int unsigned", nullable: false),
                    RoomAmount = table.Column<uint>(type: "int unsigned", nullable: false),
                    Grade = table.Column<uint>(type: "int unsigned", nullable: false),
                    Evaluate = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pictures = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Videos = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orders_Room_GuestRoomID",
                        column: x => x.GuestRoomID,
                        principalTable: "Room",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReserveOrders",
                columns: table => new
                {
                    ID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    RoomID = table.Column<uint>(type: "int unsigned", nullable: false),
                    UserID = table.Column<uint>(type: "int unsigned", nullable: false),
                    LockStartTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    LockEndTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReserveOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReserveOrders_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReserveOrders_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReserveOrders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CheckInRecords",
                columns: table => new
                {
                    ID = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReserveOrderID = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    RoomID = table.Column<uint>(type: "int unsigned", nullable: false),
                    ResidentInformation = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CheckInTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    CheckOutTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Deposit = table.Column<uint>(type: "int unsigned", nullable: false),
                    ExtraExpense = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CheckInRecords_ReserveOrders_ReserveOrderID",
                        column: x => x.ReserveOrderID,
                        principalTable: "ReserveOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckInRecords_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Administrations_Account",
                table: "Administrations",
                column: "Account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Awards_UserID",
                table: "Awards",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInRecords_ReserveOrderID",
                table: "CheckInRecords",
                column: "ReserveOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInRecords_RoomID",
                table: "CheckInRecords",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyGroups_GroupName",
                table: "CompanyGroups",
                column: "GroupName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInstanceHotelInstance_HotelInstancesID",
                table: "EmployeeInstanceHotelInstance",
                column: "HotelInstancesID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInstances_EmployeeName_IdentityCardType_IdentityCard~",
                table: "EmployeeInstances",
                columns: new[] { "EmployeeName", "IdentityCardType", "IdentityCardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelInstances_CompanyGroupID",
                table: "HotelInstances",
                column: "CompanyGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GuestRoomID",
                table: "Orders",
                column: "GuestRoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Platform_PlatOrderNumber",
                table: "Orders",
                columns: new[] { "Platform", "PlatOrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserID",
                table: "Orders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UUID",
                table: "Orders",
                column: "UUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReserveOrders_OrderID",
                table: "ReserveOrders",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveOrders_RoomID",
                table: "ReserveOrders",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveOrders_UserID",
                table: "ReserveOrders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Room_Address_Title",
                table: "Room",
                columns: new[] { "Address", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Room_HotelInstanceID",
                table: "Room",
                column: "HotelInstanceID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Account",
                table: "Users",
                column: "Account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IDNumber",
                table: "Users",
                column: "IDNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrations");

            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "CheckInRecords");

            migrationBuilder.DropTable(
                name: "EmployeeInstanceHotelInstance");

            migrationBuilder.DropTable(
                name: "ReserveOrders");

            migrationBuilder.DropTable(
                name: "EmployeeInstances");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "HotelInstances");

            migrationBuilder.DropTable(
                name: "CompanyGroups");
        }
    }
}
