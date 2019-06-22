import sklearn
from sklearn import metrics
from sklearn.cluster import KMeans
from sklearn.cluster import AgglomerativeClustering
from sklearn.cluster import AffinityPropagation
from sklearn.tree import DecisionTreeClassifier
from sklearn.cluster import DBSCAN
from sklearn.tree.export import export_text
import numpy as np
import sys
from sklearn.metrics import classification_report
from sklearn import preprocessing
import Orange
from Orange import data, distance
from Orange.clustering import hierarchical

data = []
leave = []
countObj = 0
countL0 = 0
countL1 = 0
P = 0
Ncluster = 0;
metric = ""
link = ""
init = ""
algMeans = ""
eps = 0
min_sampl = 0
damp = 0
maxiter = 0
convergiter = 0

resultFile =  str(sys.argv[1])
paramFile = str(sys.argv[2])
sourceFile = str(sys.argv[3])
normalizParam = str(sys.argv[4])
#sourceFile = "C:\\Users\\Анастасия\\source\\repos\\PredictPlayers\\PredictPlayers\\bin\\Debug\\Save\\Data.txt"
#paramFile="C:\\Users\\Анастасия\\source\\repos\\PredictPlayers\\PredictPlayers\\bin\\Debug\\Params.txt"
#resultFile="C:\\Users\\Анастасия\\source\\repos\\PredictPlayers\\PredictPlayers\\bin\\Debug\\ResultClustering.txt"
#normalizParam = "1"

def Normal(X):
    maxs = []
    mins = []
    for i in range(14):
        minVal = sys.float_info.max
        maxVal = 0
        for j in range(len(X)):
            if X[j][i] < minVal:
                minVal = X[j][i]
            if X[j][i] > maxVal:
                maxVal = X[j][i]
        maxs.append(maxVal)
        mins.append(minVal)

    for i in range(len(X)):
        for j in range(14):
            if maxs[j] != 0:
                X[i][j] = (X[i][j]-mins[j])/(maxs[j]-mins[j])
            
    return X
            
    

def Quality(labels,leave,metka):
    labLeave = []
    for i in range(0, len(labels)):
        mass = []
        mass.append(labels[i])
        mass.append(leave[i])
        labLeave.append(mass)
    labLeave.sort(key=KeyLeave)
    ind = 0
    Q = 0
    B = 0
    for m in metka:
        count = 0
        l1 = 0
        l0 = 0
        while ind < len(labLeave) and labLeave[ind][0] == m:
            count+=1
            if labLeave[ind][1] == 1:
                l1+=1
            else: l0+=1
            ind+=1
        try:    
            q = min(l1,l0)/count
        except:
            q = 0
        try:
            b = abs(P - l0/(l0+l1))
        except:
            b = 0
        Q += q
        B += b
    return Q/len(metka),B/len(metka)

def KeyLeave(value):
    return value[0]
def KeyLabel(value):
    return value[1]

#считывание данных
firstLine = 1
for line in open(sourceFile).read().split("\n"):
        if(firstLine != 1):
            arr = []
            i = 1
            for val in line.split(";"):
                if(i != 15):
                    arr.append(float(val))
                elif(i == 15):
                    l = int(val)
                    leave.append(l)
                    if(l == 0):
                        countL0+=1
                    else: countL1+=1
                    countObj+=1
                i+=1;
            data.append(arr)
        else: firstLine = 0;
        
P = countL0/countObj

file = open(paramFile,"r")
alg = file.readline().rstrip()
if(alg == "H"):
    Ncluster = int(file.readline())
    metric = file.readline().rstrip()
    link = file.readline().rstrip()
elif(alg == "K"):
    Ncluster = int(file.readline())
    initMeans = file.readline().rstrip()
    algMeans = file.readline().rstrip()
elif(alg == "D"):
    eps = float(file.readline().rstrip())
    min_sampl = int(file.readline().rstrip())
elif(alg == "A"):
    damp = float(file.readline().rstrip())
    maxiter = int(file.readline().rstrip())
    convergiter = int(file.readline().rstrip())
file.close()
        
#кластеризация
X = np.array(data)
if(normalizParam == "1"):
    X = Normal(X);

if(alg == "H"):
    clustering = AgglomerativeClustering(n_clusters=Ncluster, affinity=metric,linkage=link).fit(X)
elif(alg == "K"):
    clustering = KMeans(n_clusters=Ncluster, init=initMeans, algorithm=algMeans).fit(X)
elif(alg == "D"):
    clustering = DBSCAN(eps=eps, min_samples=min_sampl).fit(X)
elif(alg == "A"):
    clustering = AffinityPropagation(convergence_iter=convergiter,damping=damp,max_iter=maxiter).fit(X)
labels = clustering.labels_
file1 = open("DBSCAN.TXT",'w')
for i in range(len(labels)):
    file1.write(str(labels[i]) + "\n")
file1.close()
metka = list(set(labels))
q,b = Quality(labels,leave,metka);
try:
    S = metrics.silhouette_score(X, labels, metric='euclidean')
except:
    S = -1

labLeave = []
for i in range(0, len(labels)):
    mass = []
    mass.append(labels[i])
    mass.append(leave[i])
    labLeave.append(mass)
labLeave.sort(key=KeyLeave)

y_true = []
y_pred = []
cluster_pred = []
ind = 0

file1 = open(resultFile,'w')
file1.write(str(len(metka)) + "\n")
for m in metka:
    l0 = 0
    l1 = 0
    while ind < len(labLeave) and labLeave[ind][0] == m:
        if labLeave[ind][1] == 1:
            l1+=1
        else: l0+=1
        ind+=1
    file1.write(str(m) + "\n")
    file1.write("l0: " + str(l0) + "\n")
    file1.write("l1: " + str(l1) + "\n")
    if(l1 > l0):
      cluster_pred.append(1)
      file1.write("metka: 1\n")
    else: 
      cluster_pred.append(0)
      file1.write("metka: 0\n")
		  

file1.close()

ind = 0
i = 0;
for m in metka:
    while ind < len(labLeave) and labLeave[ind][0] == m:
        y_true.append(labLeave[ind][1])
        if cluster_pred[i] == 0:
            y_pred.append(0)
        else: y_pred.append(1)
        ind+=1
    i+=1

_labels = [0,1]
target_names = ['Остался','Ушел']

file = open(resultFile, 'a')
rep = classification_report(y_true, y_pred, labels=_labels, target_names=target_names)
file.write(rep)
file.close();

file = open(resultFile, 'a')
file.write(str(q) + "\n")
file.write(str(b) + "\n")
file.write(str(S))
file.close()

#file = open("Clusters.txt", 'w')
#for l in labels:
#    file.write(str(l) + "\n")
#file.close()

summa = []
for i in range(14):
    summa.append(0)

data_ = []
data_normal = []

i = 0
for d in data:
    mass = []
    mass.append(d)
    mass.append(labels[i])
    i+=1
    data_.append(mass)
data_.sort(key=KeyLabel)

i = 0
for user in X:
    mass = []
    mass.append(user)
    mass.append(labels[i])
    i+=1
    data_normal.append(mass)
data_normal.sort(key=KeyLabel)

'''file = open("Players.txt", 'w')
for player in data_:
    i = 0
    for val in player:
        if i == 0:
            for v in val:
                file.write(str(v) + " ")
            i+=1
        else:
            file.write(str(val) + "\n")
file.close()'''

file = open("Players.txt", 'w')
for player in data_normal:
    i = 0
    for val in player:
        if i == 0:
            for v in val:
                file.write(str(v) + " ")
            i+=1
        else:
            file.write(str(val) + "\n")
file.close()
    

ind = 0
count = 0
file = open("AVERAGE.txt", 'w')
file.write(str(len(metka)) + "\n")
for m in metka:
    while ind < len(data_) and data_[ind][1] == m:
        for i in range(14):
            summa[i] += data_[ind][0][i]
        ind+=1
        count+=1
    for i in range(14):
        try:
            summa[i] = summa[i]/count
        except:
            summa[i] = 0
        file.write(str(round(summa[i],3)) + " ")
    file.write(str(m))
    file.write("\n")
    count = 0
    for i in range(14):
        summa[i] = 0
file.close()  
 
ind = 0
count = 0
file = open("AVERAGE_N.txt", 'w')
file.write(str(len(metka)) + "\n")
for m in metka:
    while ind < len(data_normal) and data_normal[ind][1] == m:
        for i in range(14):
            summa[i] += data_normal[ind][0][i]
        ind+=1
        count+=1
    for i in range(14):
        try:
            summa[i] = summa[i]/count
        except:
            summa[i] = 0
        file.write(str(round(summa[i],3)) + " ")
    file.write(str(m))
    file.write("\n")
    count = 0
    for i in range(14):
        summa[i] = 0
file.close()  

#file = open("Clusters.txt", 'w')
#for d in data_:
#    file.write(str(d[0]))
#file.close()

file = open("LabelsPlayers.txt", 'w')
for l in labels:
    file.write(str(l) + "\n")
file.close()

