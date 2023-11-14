docker-compose -p hrm build --build-arg ENV_FILE=docker-dev.env web
docker tag hrm_web:latest jrankin312/hrm_web:latest
docker push jrankin312/hrm_web:latest
pause