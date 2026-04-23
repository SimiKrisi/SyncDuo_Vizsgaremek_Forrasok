<?php

include __DIR__ . '/constants.php';

// --- JSON beolvasása és validálása beszúráshoz ---
function parseAndValidateJsonForInsert(array $allowedFields): array
{
    $input = getJsonInput();
    $columns = [];
    $placeholders = [];
    $values = [];

    foreach ($input as $key => $value) {
        if (!in_array($key, $allowedFields)) {
            throw new Exception("Field not allowed: $key");
        }
        $columns[] = $key;
        $placeholders[] = ":" . $key;
        $values[$key] = $value;
    }
    if (empty($columns)) {
        throw new Exception("No fields to insert");
    }
    return [
        "columns"      => $columns,
        "placeholders" => $placeholders,
        "values"       => $values
    ];
}

// --- JSON beolvasása és validálása frissítéshez (egységesen id-t vár) ---
function parseUpdateJson(array $allowedFields, string $idField = 'id'): array
{
    $input = getJsonInput();
    if (!isset($input[$idField])) {
        throw new Exception("Missing id");
    }
    $id = $input[$idField];
    unset($input[$idField]);
    if (empty($input)) {
        throw new Exception("No fields to update");
    }
    $setParts = [];
    $values = [];
    foreach ($input as $key => $value) {
        if (!in_array($key, $allowedFields)) {
            throw new Exception("Field not allowed: $key");
        }
        $setParts[] = "$key = :$key";
        $values[$key] = $value;
    }
    $values[$idField] = $id;
    return [
        "set"    => implode(", ", $setParts),
        "values" => $values,
        "user_id"    => $id
    ];
}
function parseUpdateJsonForDailys(array $allowedFields, string $idField = 'dailyc_id'): array
{
    $input = getJsonInput();
    if (!isset($input[$idField])) {
        throw new Exception("Missing id");
    }
    $id = $input[$idField];
    unset($input[$idField]);
    if (empty($input)) {
        throw new Exception("No fields to update");
    }
    $setParts = [];
    $values = [];
    foreach ($input as $key => $value) {
        if (!in_array($key, $allowedFields)) {
            throw new Exception("Field not allowed: $key");
        }
        $setParts[] = "$key = :$key";
        $values[$key] = $value;
    }
    $values[$idField] = $id;
    return [
        "set"    => implode(", ", $setParts),
        "values" => $values,
        "dailyc_id"    => $id
    ];
}
// --- Oszlopok validálása és összeállítása SELECT-hez ---
function parseAndValidateColumnsForSelect(array $allowedFields): string
{
    $columns = $_GET['columns'] ?? '';
    if (empty($columns)) {
        // Ha nincs megadva, akkor az összes engedélyezett oszlopot használjuk
        return implode(", ", $allowedFields);
    }
    
    $requestedColumns = explode(',', $columns);
    $validatedColumns = [];
    
    foreach ($requestedColumns as $column) {
        $column = trim($column);
        if (!in_array($column, $allowedFields)) {
            throw new Exception("Column not allowed: $column");
        }
        $validatedColumns[] = $column;
    }
    
    if (empty($validatedColumns)) {
        throw new Exception("No valid columns specified");
    }
    
    return implode(", ", $validatedColumns);
}

// --- User mezők speciális validációja (email, display_name) ---
function validateUserFields(array $values, array $bannedPhrases): void
{
    foreach ($values as $key => $value) {
        if ($key === 'email') {
            if (!filter_var($value, FILTER_VALIDATE_EMAIL)) {
                throw new Exception("Invalid email format");
            }
            foreach ($bannedPhrases as $phrase) {
                if (stripos($value, $phrase) !== false) {
                    throw new Exception("Email contains forbidden phrase: $phrase");
                }
            }
        }
        if ($key === 'display_name') {
            if (!preg_match('/^[a-zA-Z0-9 _-]{3,25}$/', $value)) {
                throw new Exception("Display name contains invalid characters");
            }
            if (containsForbiddenContent($value)) {
                throw new Exception("Display name contains forbidden content");
            }
            foreach ($bannedPhrases as $phrase) {
                if (stripos($value, $phrase) !== false) {
                    throw new Exception("Display name contains forbidden phrase: $phrase");
                }
            }
        }
    }
}

// --- JSON beolvasása (újrafelhasználható) ---
function getJsonInput(): array
{
    $input = json_decode(file_get_contents("php://input"), true);
    if (!$input) {
        throw new Exception("No JSON received");
    }
    return $input;
}

// --- Hibaválasz küldése ---
function sendErrorResponse(Exception $e): void
{
    echo json_encode([
        "status" => "error",
        "message" => $e->getMessage()
    ]);
}

// --- Egységes GET paraméter kinyerése id-hoz ---
function getIdFromRequest(): ?string
{
    if (isset($_GET['id'])) {
        return (string)$_GET['id'];
    }
    if (isset($_GET['user_id'])) {
        return (string)$_GET['user_id'];
    }
    return null;
}

// --- Sikeres beszúrás válasza ---
function executeAndRespondForInsert(PDO $pdo, string $sql, array $values): void
{
    $stmt = $pdo->prepare($sql);
    $stmt->execute($values);
    echo json_encode([
        "status"        => "ok",
        "affected_rows" => $stmt->rowCount()
    ]);
}

// --- SQL végrehajtás és válasz törléshez ---
function executeAndRespondForDelete(PDO $pdo, string $sql, array $params): void
{
    $stmt = $pdo->prepare($sql);
    $stmt->execute($params);
    echo json_encode([
        "status"        => "ok",
        "affected_rows" => $stmt->rowCount()
    ]);
}

// --- SQL végrehajtás és válasz lekérdezéshez ---
function executeAndRespondForSelect(PDO $pdo, string $sql, array $params = []): void
{
    $stmt = $pdo->prepare($sql);
    $stmt->execute($params);
    $result = $stmt->fetchAll();
    echo json_encode([
        "status" => "ok",
        "data"   => $result
    ]);
}

// --- SQL végrehajtás és válasz frissítéshez ---
function executeAndRespondForUpdate(PDO $pdo, string $sql, array $values): void
{
    $stmt = $pdo->prepare($sql);
    $stmt->execute($values);
    echo json_encode([
        "status" => "ok",
        "affected_rows" => $stmt->rowCount()
    ]);
}

// --- Szöveg normalizálása (csak leetspeak csere, nem törlünk mindent!) ---
function normalizeText($text) {
    $text = strtolower($text);

    $map = [
        '@' => 'a', '4' => 'a',
        '3' => 'e',
        '1' => 'i', '!' => 'i',
        '0' => 'o',
        '$' => 's'
    ];

    return strtr($text, $map);
}

// --- Tiltott tartalom ellenőrzése ---
function containsForbiddenContent($text) {
    global $patterns, $roots;
    $normalized = normalizeText($text);

    // Regex találat
    foreach ($patterns as $pattern) {
        if (preg_match($pattern, $normalized)) {
            return true;
        }
    }

    // Gyökerek pontos tartalmazása
    foreach ($roots as $root) {
        if (strpos($normalized, $root) !== false) {
            return true;
        }
    }

    return false;
}

// --- API kulcs validálása ---
function validateApiKey(): string
{
    global $gameApiKey, $adminApiKey;
    
    $apiKey = $_SERVER['HTTP_X_API_KEY'] ?? null;
    if (!$apiKey) {
        throw new Exception("Missing API key");
    }
    
    if ($apiKey === $gameApiKey) {
        return 'game';
    } elseif ($apiKey === $adminApiKey) {
        return 'admin';
    } else {
        throw new Exception("Invalid API key");
    }
}

// --- Engedélyezett select mezők kiválasztása API kulcs alapján ---
function getAllowedSelectFields(string $module, string $apiKeyType): array
{
    global $allowedUserSelectGame, $allowedUserSelectAdmin,
           $allowedAchievementSelectGame, $allowedAchievementSelectAdmin,
           $allowedDailyCRSelectGame, $allowedDailyCRSelectAdmin,
           $allowedSpeedrunSelectGame, $allowedSpeedrunSelectAdmin,
           $allowedShopSelectGame, $allowedShopSelectAdmin,
           $allowedDailysSelectGame, $allowedDailysSelectAdmin,
           $allowedStatsSelectGame, $allowedStatsSelectAdmin;
    
    $fields = [
        'user' => ['game' => $allowedUserSelectGame, 'admin' => $allowedUserSelectAdmin],
        'achievement' => ['game' => $allowedAchievementSelectGame, 'admin' => $allowedAchievementSelectAdmin],
        'dailycrecord' => ['game' => $allowedDailyCRSelectGame, 'admin' => $allowedDailyCRSelectAdmin],
        'speedrun' => ['game' => $allowedSpeedrunSelectGame, 'admin' => $allowedSpeedrunSelectAdmin],
        'shop' => ['game' => $allowedShopSelectGame, 'admin' => $allowedShopSelectAdmin],
        'dailychallenges' => ['game' => $allowedDailysSelectGame, 'admin' => $allowedDailysSelectAdmin],
        'stats' => ['game' => $allowedStatsSelectGame, 'admin' => $allowedStatsSelectAdmin],
    ];
    
    if (!isset($fields[$module][$apiKeyType])) {
        throw new Exception("Invalid module or API key type");
    }
    
    return $fields[$module][$apiKeyType];
}
