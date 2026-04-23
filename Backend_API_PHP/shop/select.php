<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';

try {
    // --- API kulcs validálása ---
    $apiKeyType = validateApiKey();
    
    // --- 1. id kinyerése GET-ből ---
    $id = getIdFromRequest();
    if (!$id) {
        throw new Exception("Missing id");
    }
    // --- 2. Engedélyezett oszlopok összeállítása ---
    $allowedFields = getAllowedSelectFields('shop', $apiKeyType);
    $columns = parseAndValidateColumnsForSelect($allowedFields);
    // --- 3. SQL összeállítása ---
    $sql = "SELECT $columns FROM user_shop_items WHERE user_id = :id";
    // --- 4. Lekérdezés és válasz ---
    executeAndRespondForSelect($pdo, $sql, ['id' => $id]);
} catch (Exception $e) {
    // --- 5. Hibakezelés ---
    sendErrorResponse($e);
}