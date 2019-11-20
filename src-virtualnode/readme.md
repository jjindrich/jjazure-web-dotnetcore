# Deploy virtual nodes on AKS

You can deploy ACI on AKS and private vnet
- https://docs.microsoft.com/en-us/azure/aks/virtual-nodes-cli
- https://docs.microsoft.com/en-us/azure/aks/virtual-nodes-cli#enable-virtual-nodes-addon

There are some limitations
- https://docs.microsoft.com/en-us/azure/aks/virtual-nodes-cli#known-limitations
- https://docs.microsoft.com/en-us/azure/container-instances/container-instances-vnet#preview-limitations

## Deploy

You can deploy with public ip

```bash
kubectl apply -f virtual-node.yaml
```

You can deploy with internal ip

```bash
kubectl apply -f virtual-node-internal.yaml
```

## Test

Service is deployed on external load balancer.

You can test it internally on AKS cluster.

```bash
kubectl run -it --rm virtual-node-test --image=debian

apt-get update && apt-get install -y curl
curl -L http://IPaddress
```

## Clean-up

```bash
kubectl delete -f virtual-node.yaml
```