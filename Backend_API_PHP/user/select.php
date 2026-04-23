<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';

try {
    // --- API kulcs validálása ---
    $apiKeyType = validateApiKey();
    
    // --- 1. Azonosító kinyerése GET-ből ---
    $id = getIdFromRequest();
    $googleId = $_GET['googleId'] ?? $_GET['google_id'] ?? null;

    if (!$id && !$googleId) {
        throw new Exception("Missing id or googleId");
    }

    // --- 2. Oszlopok validálása és összeállítása ---
    $allowedFields = getAllowedSelectFields('user', $apiKeyType);
    $columns = parseAndValidateColumnsForSelect($allowedFields);

    // --- 3. SQL összeállítása ---
    if ($googleId !== null) {
        $sql = "SELECT $columns FROM users WHERE google_id = :google_id";
        $params = ['google_id' => $googleId];
    } else {
        $sql = "SELECT $columns FROM users WHERE user_id = :id";
        $params = ['id' => $id];
    }

    // --- 4. Lekérdezés és válasz ---
    executeAndRespondForSelect($pdo, $sql, $params);
} catch (Exception $e) {
    // --- 5. Hibakezelés ---
    sendErrorResponse($e);
}
