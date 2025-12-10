FROM php:8.1-apache

RUN apt-get update && apt-get install -y \
    libzip-dev \
    libpng-dev \
    libjpeg-dev \
    libfreetype6-dev \
    libssl-dev \
    unzip \
    git \
    && docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install -j$(nproc) gd zip pdo_mysql mysqli sockets bcmath

# Включаем mod_rewrite для Apache
RUN a2enmod rewrite

# Копируем исходный код в контейнер
COPY . /var/www/html/

# Устанавливаем права
RUN chown -R www-data:www-data /var/www/html