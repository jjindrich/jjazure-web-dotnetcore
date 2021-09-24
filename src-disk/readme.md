# Run Windows container on AKS with persistent disk

## Build docker image locally

Switch Docker to Windows Containers. Build image and push to Azure Docker Repository

```ps
docker build . -t winserver

docker login jjcontainers.azurecr.io
docker tag winserver jjcontainers.azurecr.io/winserver
docker push jjcontainers.azurecr.io/winserver
```

## Deploy to AKS

```ps
kubectl apply -f azure-disk.yaml
kubectl apply -f azure-windows-disk.yaml

kubectl describe pod winserver
kubectl get pods
```

Test drive E: on windows container

```ps
kubectl logs winserver
kubectl exec -it winserver -- powershell ls e:
```

Test mount on linux container

```ps
kubectl get pods -o wide
kubectl exec -it jjlinuxdisk -- sh touch /mnt/azure/jj
```

Drain node to see what happen with disks and check content
``ps
kubectl get pods -o wide
kubectl drain aks-agentpool-79756367-vmss000079 --ignore-daemonsets --delete-local-data
``

## Clean-up

```ps
kubectl delete pod winserver
```
