-- SQLite
UPDATE Carts
SET UpdatedAt = datetime('now', '-4 days')
WHERE Id = 1;