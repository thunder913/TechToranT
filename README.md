# TechToranT Menu

**ASP .NET Core 5.0 Web Application Project**

**The restaurant Menu project** is a project made for the Softuni **ASP.NET Core** course. The website is a business idea for **fully automated online restaurant menu**, where clients can **order through their smartphones** and waiters and cooks **receive their order immediately**.

## **Features**

This website allows restaurant to go **full online**, making it extremely easy for customers to order from their phones and the staff to track all the orders.

#### **Guest**

Everyone who visits the site is a guest.
Guests can only view the menu and see all the information about the dishes and drinks, which are offered.

#### **Client**

Whenever a user creats an account he becomes a client.
Clients have many functionalities:
* Add an item to the basket;
* View his own basket and  submit orders;
* See all the orders he has made (with their statuses) and if the order is not approved by the staff, the client can cancel it;
* Add comments on dishes and like/dislike other comments;

#### **Staff**

Waiters and chefs are considered staff.
They have all the functionalities like the client, but also many more.

#### **Cook**
Every cook has a special page for him, where he can:

* See all the new orders (that are accepted by the waiters) and view all the details about them;
* Accept an order and change its status to "Cooking"
* Upon accepting an order, all the dishes and drinks sort themselves in the correct tab with their name and count;
* When he cooks a dish, he can indicate it to the waiters by a button click and it indicates them immediately;

#### **Waiter**
Every waiter has a special page for him, where he can:
* See when a client makes an order, view all the details about the order and accept it;
* Once accepted, the order is added to the "Active orders" table, where he can see all *his* active order with their status and the cooked percentage;
* Whenever a cook makes a dish, the waiter can see it in his "Pickup" tab, with information about the table, client name and the dishes name;

#### **Administrator**

Administrators can:

* Add/Remove dishes and drinks;
* Edit dishes/drinks by changing their prices, names and photos;
* See all the registered users, edit their accounts and change their roles;
* View all the orders made and orders of a specific user;
* See analysis of the sales and the staff;
* Create amd manage all the promo codes;
* Manage the tables, change their capacity and get a new random code for each table;


## :hammer_and_wrench: Built With
- ASP.NET Core 5.0
- Entity Framework (EF) Core 5.0
- Razor View Engine
- ASP.NET Identity System
- Repository Pattern
- Web API and AJAX
- SignalR
- Auto Mapper
- xUnit
- SendGrid
- Javascript
- Bootstrap
- jQuery
- jQuery Datatables
- Bootstrap-select
- Bootstrap-notify
- Animate.css
- Chart.js
- Facebook for developers
- Google for developers



### Using ASP.NET Core 5.0 Template by : Nikolay Kostov https://github.com/NikolayIT/ASP.NET-MVC-Template
