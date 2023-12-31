# Unity_LargeScaleSearchTarget
大规模目标时，常规遍历搜索、KDTree搜索、格子搜索以及八叉树搜索的性能对比。

# 多大规模算大规模？
常规搜索方式下耗时很久的就算。假如只搜索一次的话，PC上5000数量起，手机上100数量起。

# 通常的搜索目标要求
* 范围内最近的1个目标
* 范围内最近的n个目标（n>1)
* 范围内的随机n个目标（n>1)

# 实验的四种搜索方式
* Normal，常规搜索，直接遍历所有目标。
* KDTreeNearest和KDTreeNearestRange，由于KDTree搜索最近的一个和搜索范围内的目标过程不一样，所以要分别进行测试。KDTRee构建是需要花费时间的，所以将搜索目标的请需求集中起来，构建好后，一次性完成所有搜索需求。
* Grid，网格搜索。从最近的格子开始一圈一圈向外围搜索。可以在O(1)的时间内更新怪物所在的格子，维护格子的性能消耗可以忽略。
* Octree，八叉树搜索。同样是将搜索请求集中起来，八叉树构建好，一次性完成所有搜索请求。实际应用过程中应该动态更新树，除非大量节点都更换了格子，否则更新的效率更高。

# 结论
* 通常Grid是最快的。当大量怪物扎堆挤入同一个格子时才会降低格子的效果。如果格子过大还可以缩小格子尺寸来解决这种情况。可以说只要内存足够，就不是问题。假如是平面1024*1024个格子，每个格子4字节（需要一个列表记录怪物），就需要4M。
* KDTreeNearest所有规模下都有稳定的表现。但是只是搜索最近的1个。搜索最近的n个目标可以用一个最大堆存储搜索到的目标，离英雄最远的在顶上，每次发现比堆顶目标更近的，就把堆顶替换掉。KDTree的优势就在于不论分布范围多大，都能处理；缺点就是节点移动后更新效率不高。
* KDTreeNearestRange搜索范围内的所有目标。搜索范围越大，遍历的节点越多就越慢。搜索范围内随机N个目标就得用这个方式。除非范围覆盖全图，否则不会比常规搜索慢。
* Octree搜索在范围较大时速度明显慢，应该是比较的次数更多（xyz三维比较）。构建比较费时间，当搜索范围很小时速度非常快。当移动改变格子的对象很少时，维护tree的时间就很少，Octree就非常适合，总体上会比kdtree快。所以Octree虽然慢了点，但是节点更新效率高，节点移动的情况下比KDtree好。
* Normal，数量比较少时，用这个方也没问题。测试机器，200个敌人，10个英雄同时搜索1次目标，耗时是5ms左右。如果分帧每帧完成2个搜索需求，就是1ms了。CPU不忙的场景完全可以接受。


# 测试条件
* 测试渣渣手机
  海思麒麟710 CPU频率	4×Cortex A73 2.2 GHz + 4×Cortex A53 1.7 GHz
* 英雄搜索方式：所有英雄同时搜索n次目标。
* 地图50x50

# 一些测试数据
| 搜索模式            | 怪物数量 | 攻击距离 | 英雄数量 | 搜索次数 | 格子大小 | 耗时（us） |
|-----------------|------|------|------|------|------|--------|
| Normal          | 100  | 20   | 10   | 1    | -    | 2642   |
| Normal          | 100  | 20   | 10   | 10   | -    | 22330  |
| Normal          | 200  | 20   | 10   | 1    | -    | 5194   |
| Normal          | 200  | 20   | 10   | 10   | -    | 35148  |
| Normal          | 1000 | 20   | 10   | 1    | -    | 14294  |
| Normal          | 1000 | 20   | 10   | 10   | -    | 104638 |
| Normal          | 3000 | 20   | 10   | 1    | -    | 41387  |
| Normal          | 3000 | 20   | 10   | 10   | -    | 460581 |
|                 |      |      |      |      |      |        |
| KDTreeNearestR. | 100  | 10   | 10   | 1    | -    | 1281   |
| KDTreeNearestR. | 100  | 10   | 10   | 10   | -    | 4290   |
| KDTreeNearestR. | 100  | 20   | 10   | 10   | -    | 10310  |
| KDTreeNearestR. | 200  | 10   | 10   | 1    | -    | 2572   |
| KDTreeNearestR. | 200  | 10   | 10   | 10   | -    | 8197   |
| KDTreeNearestR. | 200  | 20   | 10   | 10   | -    | 20390  |
| KDTreeNearestR. | 1000 | 10   | 10   | 1    | -    | 6230   |
| KDTreeNearestR. | 1000 | 10   | 10   | 10   | -    | 20939  |
| KDTreeNearestR. | 1000 | 20   | 10   | 10   | -    | 57858  |
| KDTreeNearestR. | 3000 | 10   | 10   | 1    | -    | 23012  |
| KDTreeNearestR. | 3000 | 10   | 10   | 10   | -    | 69803  |
| KDTreeNearestR. | 3000 | 20   | 10   | 10   | -    | 240204 |
| KDTreeNearestR. | 3000 | 3    | 10   | 10   | -    | 20978  |
|                 |      |      |      |      |      |        |
| KDTreeNearest   | 100  | 10   | 10   | 1    | -    | 1693   |
| KDTreeNearest   | 100  | 10   | 10   | 10   | -    | 8776   |
| KDTreeNearest   | 100  | 20   | 10   | 10   | -    | 8862   |
| KDTreeNearest   | 200  | 10   | 10   | 1    | -    | 3160   |
| KDTreeNearest   | 200  | 10   | 10   | 10   | -    | 13833  |
| KDTreeNearest   | 200  | 20   | 10   | 10   | -    | 13828  |
| KDTreeNearest   | 1000 | 10   | 10   | 1    | -    | 8049   |
| KDTreeNearest   | 1000 | 10   | 10   | 10   | -    | 18813  |
| KDTreeNearest   | 1000 | 20   | 10   | 10   | -    | -      |
| KDTreeNearest   | 3000 | 10   | 10   | 1    | -    | 18144  |
| KDTreeNearest   | 3000 | 10   | 10   | 10   | -    | 23922  |
| KDTreeNearest   | 3000 | 20   | 10   | 10   | -    | -      |
| KDTreeNearest   | 3000 | 3    | 10   | 10   | -    | 24854  |
|                 |      |      |      |      |      |        |
| Grid            | 100  | 10   | 10   | 1    | 2    | 490    |
| Grid            | 100  | 10   | 10   | 10   | 2    | 2630   |
| Grid            | 100  | 20   | 10   | 10   | 2    | 2623   |
| Grid            | 100  | 20   | 10   | 10   | 5    | 3066   |
| Grid            | 200  | 10   | 10   | 1    | 2    | 464    |
| Grid            | 200  | 10   | 10   | 10   | 2    | 2540   |
| Grid            | 200  | 20   | 10   | 10   | 2    | 2519   |
| Grid            | 200  | 20   | 10   | 10   | 5    | 4156   |
| Grid            | 1000 | 10   | 10   | 1    | 2    | 487    |
| Grid            | 1000 | 10   | 10   | 10   | 2    | 2387   |
| Grid            | 1000 | 20   | 10   | 10   | 2    | 2392   |
| Grid            | 1000 | 20   | 10   | 10   | 5    | 7434   |
| Grid            | 3000 | 10   | 10   | 1    | 2    | 381    |
| Grid            | 3000 | 10   | 10   | 10   | 2    | 3294   |
| Grid            | 3000 | 20   | 10   | 10   | 2    | 2252   |
| Grid            | 3000 | 20   | 10   | 10   | 5    | 22944  |
| Grid            | 3000 | 3    | 10   | 10   | 2    | 1381   |
| |  |3    |    |    |     |    |
| 以下数据在PC上测得|  |3    |    |    |     |    |
| Normal          | 3000 | 10 | 10 | 1  | -    | 20,000  |
| KDTreeNearestR. | 3000 | 10 | 10 | 1  | -    | 16,000  |
| KDTreeNearest   | 3000 | 10 | 10 | 1  | -    | 14,000  |
| Grid            | 3000 | 10 | 10 | 1  | 2    | 500     |
| Normal          | 3000 | 3  | 10 | 1  | -    | 22,000  |
| KDTreeNearestR. | 3000 | 3  | 10 | 1  | -    | 8,000   |
| KDTreeNearestR. | 3000 | 3  | 10 | 10 | -    | 12,551  |
| KDTreeNearest   | 3000 | 3  | 10 | 1  | -    | 8,000   |
| KDTreeNearest   | 3000 | 3  | 10 | 10 |      | 12,151  |
|                 |      |    |    |    |      |         |
| Grid            | 3000 | 3  | 10 | 1  | 2    | 500     |
|                 |      |    |    |    |      |         |
| Octree          | 100  | 10 | 10 | 1  | -    | 1,500   |
| Octree          | 100  | 10 | 10 | 10 | -    | 6,000   |
| Octree          | 100  | 20 | 10 | 10 | -    | 10,000  |
| Octree          | 200  | 10 | 10 | 1  | -    | 3,000   |
| Octree          | 200  | 10 | 10 | 10 | -    | 5,000   |
| Octree          | 200  | 20 | 10 | 10 | -    | 16,000  |
| Octree          | 1000 | 10 | 10 | 1  | -    | 17,000  |
| Octree          | 1000 | 10 | 10 | 10 | -    | 35,000  |
| Octree          | 1000 | 20 | 10 | 10 | -    | 90,000  |
| Octree          | 3000 | 10 | 10 | 1  | -    | 46,000  |
| Octree          | 3000 | 10 | 10 | 10 | -    | 110,000 |
| Octree          | 3000 | 20 | 10 | 10 | -    | 340,000 |
| Octree          | 3000 | 3  | 10 | 10 | -    | 35,454  |
| Octree          | 3000 | 3  | 10 | 10 | 均匀分布 | 40,000  |
| Octree          | 3000 | 3  | 10 | 1  | -    | 33,004  |



![1](./img/1.png)
![2](./img/2.png)
![3](./img/3.png)
![3](./img/4.png)
