FROM mcr.microsoft.com/devcontainers/dotnet:9.0

RUN apt-get update && apt-get install -y \
    nodejs \
    npm \
    redis-server \
    postgresql \
    postgresql-contrib \
    && rm -rf /var/lib/apt/lists/*

COPY init-db.sh /tmp/init-db.sh
RUN chmod +x /tmp/init-db.sh && \
    service postgresql start && \
    su - postgres -c "bash /tmp/init-db.sh" && \
    service postgresql stop && \
    rm /tmp/init-db.sh

COPY start-services.sh /start-services.sh
RUN chmod +x /start-services.sh
