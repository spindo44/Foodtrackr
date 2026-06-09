CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);


CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);


CREATE TABLE "FoodItems" (
    "FoodId" TEXT NOT NULL CONSTRAINT "PK_FoodItems" PRIMARY KEY,
    "FoodName" TEXT NOT NULL,
    "EnergyKj" REAL NULL,
    "EnergyKcal" REAL NULL,
    "ProteinG" REAL NULL,
    "FatG" REAL NULL,
    "CarbohydrateG" REAL NULL,
    "FibreTotalG" REAL NULL,
    "SugarsG" REAL NULL,
    "StarchG" REAL NULL,
    "AlcoholG" REAL NULL,
    "WaterG" REAL NULL,
    "FatSaturatedG" REAL NULL,
    "FatMonoG" REAL NULL,
    "FatPolyG" REAL NULL,
    "FatTransG" REAL NULL,
    "Omega3G" REAL NULL,
    "Omega6G" REAL NULL,
    "CholesterolMg" REAL NULL,
    "CalciumMg" REAL NULL,
    "IronMg" REAL NULL,
    "SodiumMg" REAL NULL,
    "PotassiumMg" REAL NULL,
    "MagnesiumMg" REAL NULL,
    "PhosphorusMg" REAL NULL,
    "ZincMg" REAL NULL,
    "SeleniumUg" REAL NULL,
    "IodideUg" REAL NULL,
    "CopperMg" REAL NULL,
    "ManganeseUg" REAL NULL,
    "VitaminCMg" REAL NULL,
    "VitaminD" REAL NULL,
    "VitaminB12Ug" REAL NULL,
    "VitaminB6Mg" REAL NULL,
    "FolateUg" REAL NULL,
    "DietaryFolateEqUg" REAL NULL,
    "ThiaminMg" REAL NULL,
    "RiboflavinMg" REAL NULL,
    "NiacinMg" REAL NULL,
    "RetinolUg" REAL NULL,
    "VitaminAUg" REAL NULL,
    "VitaminEMg" REAL NULL,
    "BetaCaroteneUg" REAL NULL,
    "CaffeineM" REAL NULL
);


CREATE TABLE "Patients" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Patients" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Age" INTEGER NOT NULL,
    "Gender" TEXT NOT NULL,
    "Ethnicity" TEXT NOT NULL,
    "WeightKg" REAL NOT NULL,
    "HeightCm" REAL NOT NULL,
    "IsMetric" INTEGER NOT NULL,
    "ActivityLevel" TEXT NOT NULL,
    "MedicalConditions" TEXT NOT NULL,
    "DietaryRestrictions" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UserId" TEXT NOT NULL
);


CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "FoodPortions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_FoodPortions" PRIMARY KEY AUTOINCREMENT,
    "FoodId" TEXT NOT NULL,
    "MeasureDescription" TEXT NOT NULL,
    "WeightGrams" REAL NOT NULL,
    "Density" REAL NULL,
    CONSTRAINT "FK_FoodPortions_FoodItems_FoodId" FOREIGN KEY ("FoodId") REFERENCES "FoodItems" ("FoodId") ON DELETE CASCADE
);


CREATE TABLE "FoodLogEntries" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_FoodLogEntries" PRIMARY KEY AUTOINCREMENT,
    "PatientId" INTEGER NOT NULL,
    "FoodId" TEXT NOT NULL,
    "FoodName" TEXT NOT NULL,
    "PortionWeightGrams" REAL NOT NULL,
    "MealType" TEXT NOT NULL,
    "LoggedAt" TEXT NOT NULL,
    "IsCustom" INTEGER NOT NULL,
    "UserId" TEXT NOT NULL,
    "EnergyKcalPer100g" REAL NULL,
    "ProteinPer100g" REAL NULL,
    "CarbsPer100g" REAL NULL,
    "FatPer100g" REAL NULL,
    "FoodItemFoodId" TEXT NULL,
    CONSTRAINT "FK_FoodLogEntries_FoodItems_FoodItemFoodId" FOREIGN KEY ("FoodItemFoodId") REFERENCES "FoodItems" ("FoodId"),
    CONSTRAINT "FK_FoodLogEntries_Patients_PatientId" FOREIGN KEY ("PatientId") REFERENCES "Patients" ("Id") ON DELETE CASCADE
);


CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");


CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");


CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");


CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");


CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");


CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");


CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");


CREATE INDEX "IX_FoodLogEntries_FoodItemFoodId" ON "FoodLogEntries" ("FoodItemFoodId");


CREATE INDEX "IX_FoodLogEntries_PatientId" ON "FoodLogEntries" ("PatientId");


CREATE INDEX "IX_FoodPortions_FoodId" ON "FoodPortions" ("FoodId");


