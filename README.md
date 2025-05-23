下載Restaurant.sql並使用SQL Server執行載入資料庫

以Visual Studio 2022 或以前版本開啟Restaurant.sln後將appsettings.json內的 "RestaurantConnection": "Data Source=伺服器名稱;Database=Restaurant;TrustServerCertificate=True;User ID=帳號;Password=密碼"
，修改為本機伺服器名稱，並創建新使用者，此處預設帳號：abc，密碼：123，修改使用權限及登入方式。
執行後預設為登入畫面，Program.cs內預設登入計時10分鐘後登出，註冊帳號時會及時在前端驗證資料庫是否有重複，並提示使用者，避免重複註冊，預設所有使用者角色皆為MB，資料庫內預設C501~C504等對應不同角色之測試帳號，並將密碼以雜湊形式進行保護，登入後navbar會顯示該角色能閱讀的權限內容，MB為一般使用者，僅能看見自己帳戶內的資料。
另外購物車功能使用Local storage的key做區分，避免在同一瀏覽器中使用不同帳戶登入顯示相同購物車內容。
