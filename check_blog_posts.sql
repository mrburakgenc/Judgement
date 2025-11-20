-- Blog yazılarını kontrol et
SELECT
    "Id",
    "Title",
    "Slug",
    "IsPublished",
    "PublishedAt",
    "CreatedAt"
FROM "BlogPosts"
ORDER BY "CreatedAt" DESC;

-- Toplam sayı
SELECT
    COUNT(*) as "TotalPosts",
    SUM(CASE WHEN "IsPublished" THEN 1 ELSE 0 END) as "PublishedPosts",
    SUM(CASE WHEN NOT "IsPublished" THEN 1 ELSE 0 END) as "DraftPosts"
FROM "BlogPosts";
