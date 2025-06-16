# MeteoLive

## Construccion de imagen docker
docker build -t meteolive .  
docker run -p 6080:6080 meteolive  

## Login and Push a DockerHub
 docker login -u [user]  
 docker tag meteolive:latest [user]/meteolive:v1.0.0  
 docker push [user]/meteolive:v1.0.0  

## Crear dos secrets en GitHub: ( GitHub -> Settings -> Secrets -> Actions )
DOCKER_USERNAME  
DOCKER_PASSWORD  