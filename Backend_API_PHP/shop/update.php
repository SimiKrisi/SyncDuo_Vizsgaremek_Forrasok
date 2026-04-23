<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';


try {
    // --- 2. JSON beolvasása és validálása ---
    $parsed = parseUpdateJson($allowedShop, 'user_id');
    // --- 3. SQL összeállítása ---
    $sql = "UPDATE user_shop_items SET {$parsed['set']} WHERE user_id = :user_id";
    // --- 4. Lekérdezés előkészítése és futtatása ---
    executeAndRespondForUpdate($pdo, $sql, $parsed["values"]);

} catch (Exception $e) {
    // --- 6. Hibakezelés ---
    sendErrorResponse($e);
}