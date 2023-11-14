docker-compose -p hrm build --build-arg ENV_FILE=docker-dev.env ws
docker tag hrm_ws:latest jrankin312/hrm_ws:latest
docker push jrankin312/hrm_ws:latest
pause